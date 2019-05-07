using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class BeginPayOrder
    {
        [FunctionName("BeginPayOrder")]
        public static void Run([CosmosDBTrigger(
            databaseName: "MobileOrders",
            collectionName: "orders",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "paid-orders-lease", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, 
            ILogger log,
            [Queue("pay-order-requests", Connection = "AzureWebJobsStorage")] ICollector<Order> outputOrders)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var document in input)
                {

                    var order = OrderManager.FromJson(document.ToString());
                    if (order.OrderStatus != "estimate-accepted") continue;
                    log.LogInformation("\n\n\n************ ATTEMPTING TO PAY ORDER *************\n\n\n");
                    order.OrderStatus = "attempting-to-pay";
                    outputOrders.Add(order);
                }
            }

        }
    }
}
