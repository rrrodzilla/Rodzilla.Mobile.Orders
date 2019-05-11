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

namespace Rodzilla.Mobile.Orders
{
    public static class Negotiate
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "orders")]SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
