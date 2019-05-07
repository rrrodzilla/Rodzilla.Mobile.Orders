using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class FinishOrder
    {
        [FunctionName("FinishOrder")]
        public static void Run([QueueTrigger("finish-order-request", Connection = "AzureWebJobsStorage")]OrderIdentifier incomingOrder, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            order.OrderStatus = "order-fulfilled";
            order.FinishTime = DateTime.Now;
        }
    }
}
