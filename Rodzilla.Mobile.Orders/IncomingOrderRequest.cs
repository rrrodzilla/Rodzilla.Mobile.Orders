using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingOrderRequest
    {
        [FunctionName("IncomingOrderRequest")]
        public static void Run([QueueTrigger("incoming-order-requests", Connection = "AzureWebJobsStorage")]Order incomingOrderRequest,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection")]out Order newOrder)
        {
            newOrder = incomingOrderRequest;
            
        }
    }
}
