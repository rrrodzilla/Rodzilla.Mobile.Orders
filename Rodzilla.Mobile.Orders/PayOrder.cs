using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class PayOrder
    {
        [FunctionName("PayOrder")]
        public static void Run([QueueTrigger("pay-order-requests", Connection = "AzureWebJobsStorage")]Order payRequestOrder, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{Id}", PartitionKey = "{CustomerId}")]Order order)
        {
            order.OrderStatus = "ready-for-fulfillment";
            log.LogInformation("\n\n\n************ PAYING ORDER WITH STRIPE *************\n\n\n");
        }
    }
}
