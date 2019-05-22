using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;

namespace Rodzilla.Mobile.Orders
{
    public static class ReceivePaymentToken
    {
        [FunctionName("ReceivePaymentToken")]
        [return:Queue("receive-new-payment-token",Connection = "AzureWebJobsStorage")]
        public static OrderTokenIdentifier Run(
            [HttpTrigger(AuthorizationLevel.Function, "post","get", Route = null)] OrderTokenIdentifier orderTokenIdentifier,
            ILogger log)
        {
            return orderTokenIdentifier;
        }
    }
}
