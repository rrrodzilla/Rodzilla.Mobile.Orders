using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingOrderEstimate
    {
        [FunctionName("IncomingOrderEstimate")]
        public static async Task Run(
            [QueueTrigger("incoming-order-estimate", Connection = "AzureWebJobsStorage")] OrderEstimate incomingEstimate,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order,
        [SignalR(HubName = "orders")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            order.EstimatedMinutes = incomingEstimate.EstimatedMinutes;
            order.Price = incomingEstimate.Price;
            order.OrderStatus = "estimated";
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "updateStatus",
                    Arguments = new object[] { order }
                });
        }
    }
}
