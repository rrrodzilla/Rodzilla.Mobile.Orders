using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class ReceiveQuestionResponse
    {
        [FunctionName("ReceiveQuestionResponse")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("mo-receive-question-response", Connection = "AzureWebJobsStorage")]OrderQuestionReply myQueueItem, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            //we want to update the order with this question reply
            OrderManager.Reply(myQueueItem.Response, ref order);
            //then we want to notify the restaurant of the response
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "updateStatus",
                    Arguments = new object[] { order }
                });

            order.OrderStatus = "requested";
        }
    }
}
