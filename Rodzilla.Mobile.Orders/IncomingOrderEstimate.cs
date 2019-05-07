using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingOrderEstimate
    {
        [FunctionName("IncomingOrderEstimate")]
        public static void Run(
            [QueueTrigger("incoming-order-estimate", Connection = "AzureWebJobsStorage")] OrderEstimate incomingEstimate,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            order.EstimatedMinutes = incomingEstimate.EstimatedMinutes;
            order.Price = incomingEstimate.Price;
            order.OrderStatus = "estimated";
        }
    }
}
