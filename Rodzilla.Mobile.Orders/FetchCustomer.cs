using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class FetchCustomer
    {
        [FunctionName("FetchCustomer")]
        [return:Queue("customer-found", Connection = "AzureWebJobsStorage")]
        public static Customer Run([QueueTrigger("get-customer", Connection = "AzureWebJobsStorage")]CustomerIdentifier customerIdentifier, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers", ConnectionStringSetting = "DBConnection", Id="{RowKey}", PartitionKey = "{StripeId}", CreateIfNotExists = true)]Customer customer,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers",
                ConnectionStringSetting = "DBConnection")]out Customer document)
        {
            document = null;
            if (customer == null)
            {
                //let's create it
                customer = new Customer(){Id = customerIdentifier.RowKey, StripeId = customerIdentifier.StripeId, IsNew = true};
                document = customer;
            }
                //let's notify whoever asked about this found customer
            log.LogInformation($"Customer Found: {customer.PhoneNumber}");
            return customer;
        }
    }
}
