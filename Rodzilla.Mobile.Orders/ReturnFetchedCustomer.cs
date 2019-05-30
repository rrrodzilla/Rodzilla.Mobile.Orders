using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rodzilla.Mobile.Orders.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class ReturnFetchedCustomer
    {
        [FunctionName("ReturnFetchedCustomer")]
        public static async Task Run([QueueTrigger("customer-found", Connection = "AzureWebJobsStorage")]Customer customer, ILogger log,
        [CosmosDB(databaseName: "MobileOrders", collectionName: "orders", ConnectionStringSetting = "DBConnection")]DocumentClient client)
        {
            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));

            //now that we have a customer, we want to see if there are any open orders for this customer so we can either let them start a new one
            var collectionUri = UriFactory.CreateDocumentCollectionUri("MobileOrders", "orders");


            var query = client.CreateDocumentQuery<Order>(collectionUri, feedOptions: new FeedOptions() { EnableCrossPartitionQuery = true })
                .Where(p => ((p.Customer.Id == customer.Id) &&
                             (p.OrderStatus != "order-fulfilled") && (p.OrderStatus != "canceled"))).AsDocumentQuery();
            //var orders = new Collection<Order>();
            Order existingOrder = null;
            while (query.HasMoreResults)
            {
                foreach (Order result in await query.ExecuteNextAsync())
                {
                    existingOrder = result;
                }
            }

            //or deal with the existing one first
            var message = (existingOrder == null) ?
                new JObject(new JProperty("customer", JObject.FromObject(customer)), new JProperty("action", "begin-order")) :
                new JObject(new JProperty("customer", JObject.FromObject(customer)), new JProperty("order", JObject.FromObject(existingOrder)), new JProperty("action", "existing-open-order"));

            await ExecutionResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(customer.PhoneNumber),
                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                parameters: message
            );
        }
    }
}
