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
    public static class AskQuestion
    {
        [FunctionName("AskQuestion")]
        [return: Queue("mo-send-question", Connection = "AzureWebJobsStorage")]
        public static OrderQuestion Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] OrderQuestion question,
            ILogger log)
        {
            return question;
        }
    }
}
