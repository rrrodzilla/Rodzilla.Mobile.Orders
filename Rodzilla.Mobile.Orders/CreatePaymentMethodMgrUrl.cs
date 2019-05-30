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
    public static class CreatePaymentMethodMgrUrl
    {
        [FunctionName("CreatePaymentMethodMgrUrl")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("mo-payment-method-url", Connection = "AzureWebJobsStorage")]Order incomingOrder, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{Id}", PartitionKey = "{Customer.Id}")]Order order)
        {
            //we need to call the url shortener service and get back a short url to pass to the user

            //https://laundryawayweb.z5.web.core.windows.net/payment-method/method/{order.Id}/{order.Customer.Id}/{order.Customer.StripeId}
            using (var client = new HttpClient())
            {
                var headers = client.DefaultRequestHeaders;
                headers.Add("x-functions-key", "nYURKnr2i/RUXydjUG7Mg99BksTZIvWbN5EPRe0a/iJQCDdMwKKa3Q==");
                var json = $"{{'target': 'https://laundryawayweb.z5.web.core.windows.net/payment-method/method/{order.Id}/{order.Customer.Id}/{order.Customer.StripeId}'}}";

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("https://4.2go2.us/api/", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                log.LogInformation(resultContent);
                //we need to notify the user via twilio
                TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                //var shortUrlVal = $"{Environment.GetEnvironmentVariable("BaseUrl")}{shortener.Current.BaseValue}";

                var message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "payment-method-needed"), new JProperty("shortUrl", resultContent));

                ExecutionResource.Create(
                    to: new Twilio.Types.PhoneNumber(order.Customer.Id),
                    from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                    pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                    parameters: message
                );
            }


        }
    }
}
