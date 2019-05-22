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
    public static class SendOrderEstimate
    {
        [FunctionName("SendOrderEstimate")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] OrderEstimate orderIdentifier,
            ILogger log,
            [Queue("incoming-order-estimate", Connection = "AzureWebJobsStorage")] IAsyncCollector<OrderEstimate> orderCollector)
        {

            await orderCollector.AddAsync(orderIdentifier);
        }
    }
}
