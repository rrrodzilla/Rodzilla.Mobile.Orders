using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class FinishOrder
    {
        [FunctionName("FinishOrder")]
        public static async Task Run([QueueTrigger("finish-order-request", Connection = "AzureWebJobsStorage")]OrderIdentifier incomingOrder, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            order.OrderStatus = "order-fulfilled";
            order.FinishTime = DateTime.Now;
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "updateStatus",
                    Arguments = new object[] { order }
                });
        }
    }
}
