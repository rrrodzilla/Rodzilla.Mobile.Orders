using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class BeginOrder
    {
        [FunctionName("BeginOrder")]
        public static async Task Run([QueueTrigger("start-order-request", Connection = "AzureWebJobsStorage")]OrderIdentifier incomingOrderIdentifier, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages)

        {
            order.OrderStatus = "order-started";
            order.StartTime = DateTime.Now;
            
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "updateStatus",
                    Arguments = new object[] { order }
                });

        }
    }
}
