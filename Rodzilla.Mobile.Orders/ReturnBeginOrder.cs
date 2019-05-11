using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rodzilla.Mobile.Orders.Models;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class ReturnBeginOrder
    {
        [FunctionName("ReturnBeginOrder")]
        public static async Task Run([QueueTrigger("begin-order", Connection = "AzureWebJobsStorage")]Customer customer, ILogger log)
        {
            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
            var message = new JObject(new JProperty("customer", JObject.FromObject(customer)), new JProperty("action", "begin-order"));
            //await ExecutionResource.CreateAsync(
            //        to: new Twilio.Types.PhoneNumber(customer.PhoneNumber),
            //        from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
            //        pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
            //        parameters: message
            //    );
        }
    }
}
