using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rodzilla.Mobile.Orders.Models;
using Twilio;
using Newtonsoft.Json.Linq;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class Ask
    {
        [FunctionName("Ask")]
        public static async Task RunAsync(
            [QueueTrigger("mo-send-question", Connection = "AzureWebJobsStorage")]OrderQuestion question,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            //string question = req.Query["question"];        
            OrderManager.Ask(question, ref order);
            order.OrderStatus = "awaiting-response";

            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
            log.LogInformation($"\n\n\n************************* SENDING QUESTION *********************\n\n\n");
            var message = new JObject(new JProperty("question", JObject.FromObject(question)), new JProperty("action", "awaiting-response"));

            await ExecutionResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(order.Customer.Id),
                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                parameters: message
            );

        }
    }
}
