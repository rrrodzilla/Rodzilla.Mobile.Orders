using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rodzilla.Mobile.Orders.Models;
using System;
using System.Net.Http;
using System.Text;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class SendReceipt
    {
        [FunctionName("SendReceipt")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("mo-send-receipt", Connection = "AzureWebJobsStorage")]Order incomingOrder, ILogger log)
        {

            using (var client = new HttpClient())
            {
                var headers = client.DefaultRequestHeaders;
                headers.Add("x-functions-key", "nYURKnr2i/RUXydjUG7Mg99BksTZIvWbN5EPRe0a/iJQCDdMwKKa3Q==");
                var json = $"{{'target': '{incomingOrder.ReceiptUrl}'}}";

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://4.2go2.us/api/", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                log.LogInformation(resultContent);

                incomingOrder.ReceiptUrl = resultContent;

                //we need to notify the user via twilio

                TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                var message = new JObject(new JProperty("order", JObject.FromObject(incomingOrder)), new JProperty("action", "order-paid"));

                ExecutionResource.Create(
                    to: new Twilio.Types.PhoneNumber(incomingOrder.Customer.Id),
                    from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                    pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                    parameters: message
                );
            }




        }
    }
}
