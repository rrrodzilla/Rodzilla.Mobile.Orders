using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingOrderRequest
    {
        [FunctionName("IncomingOrderRequest")]
        public static Task Run([QueueTrigger("incoming-order-requests", Connection = "AzureWebJobsStorage")]Order incomingOrderRequest,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers",Id = "{Customer.Id}", PartitionKey = "{Customer.StripeId}",ConnectionStringSetting = "DBConnection")] Customer customer,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection")]out Order newOrder,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            newOrder = incomingOrderRequest;
            //let's give this order the full customer info as it was at the time of ordering
            newOrder.Customer = customer;

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "addOrder",
                    Arguments = new object[] { incomingOrderRequest }
                });

        }
    }
}
