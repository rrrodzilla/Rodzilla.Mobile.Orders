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
    public static class FwdUrl
    {
        [FunctionName("FwdUrl")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{code}")] HttpRequest req,
            ILogger log, string code,
            [Table("shortUrls", "mobile-orders", "{code}", Connection = "AzureWebJobsStorage")] ShortUrl shortUrl)
        {            
            return new RedirectResult(shortUrl.TargetUrl, true);
        }
    }
}
