using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class AcceptOrderEstimate
    {
        [FunctionName("AcceptOrderEstimate")]
        public static void Run([QueueTrigger("accept-order-estimate", Connection = "AzureWebJobsStorage")] OrderIdentifier incomingOrder,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            order.OrderStatus = "estimate-accepted";
        }
    }
}
