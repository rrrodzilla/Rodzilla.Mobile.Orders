using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingCancelOrderRequest
    {
        [FunctionName("IncomingCancelOrderRequest")]
        public static void Run([QueueTrigger("mo-cancel-order-request", Connection = "AzureWebJobsStorage")]OrderIdentifier cancelRequest, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            order.OrderStatus = "canceled";
        }
    }
}
