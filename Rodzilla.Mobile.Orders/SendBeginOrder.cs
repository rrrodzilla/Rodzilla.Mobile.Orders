using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class SendBeginOrder
    {
        [FunctionName("SendBeginOrder")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] OrderIdentifier orderIdentifier,
            ILogger log,
            [Queue("start-order-request", Connection = "AzureWebJobsStorage")] IAsyncCollector<OrderIdentifier> orderCollector)
        {
            await orderCollector.AddAsync(new OrderIdentifier(){OrderId = orderIdentifier.OrderId, CustomerId = orderIdentifier.CustomerId});
        }
    }
}
