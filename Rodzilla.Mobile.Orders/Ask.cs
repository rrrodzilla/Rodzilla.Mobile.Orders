using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class Ask
    {
        [FunctionName("Ask")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "ask/{id}/{key}")] HttpRequest req,
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{id}", PartitionKey = "{key}")]Order order)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string question = req.Query["question"];

            OrderManager.Ask(question, ref order);

        }
    }
}
