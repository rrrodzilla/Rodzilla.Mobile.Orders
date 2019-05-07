using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class BeginOrder
    {
        [FunctionName("BeginOrder")]
        public static void Run([QueueTrigger("start-order-request", Connection = "AzureWebJobsStorage")]OrderIdentifier incomingOrderIdentifier, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)            
        {
            order.OrderStatus = "order-started";
            order.StartTime = DateTime.Now;
        }
    }
}
