using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class SendOrder
    {
        [FunctionName("SendOrder")]
        public static Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]  object message ,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "addOrder",
                    Arguments = new object[] { message }
                });
        }
    }
}
