using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class FetchIncomingOrders
    {
        [FunctionName("FetchIncomingOrders")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            [CosmosDB(databaseName: "MobileOrders", collectionName: "orders", ConnectionStringSetting = "DBConnection")]DocumentClient client)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri("MobileOrders", "orders");


            var query = client.CreateDocumentQuery<Order>(collectionUri, feedOptions: new FeedOptions() { EnableCrossPartitionQuery = true })
                .Where(p => (p.OrderStatus == "requested" || p.OrderStatus == "estimated" || p.OrderStatus == "awaiting-response"))
                .AsDocumentQuery();

            var orders = new Collection<Order>();

            while (query.HasMoreResults)
            {
                foreach (Order result in await query.ExecuteNextAsync())
                {
                    log.LogInformation(result.OrderStatus);
                    orders.Add(result);
                }
            }
            return new JsonResult(orders, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }
    }
}