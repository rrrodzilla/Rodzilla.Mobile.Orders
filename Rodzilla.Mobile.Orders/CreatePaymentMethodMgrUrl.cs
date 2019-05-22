using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rodzilla.Mobile.Orders.Models;
using System;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class CreatePaymentMethodMgrUrl
    {
        [FunctionName("CreatePaymentMethodMgrUrl")]
        public static void Run([QueueTrigger("mo-payment-method-url", Connection = "AzureWebJobsStorage")]Order incomingOrder, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{Id}", PartitionKey = "{CustomerId}")]Order order,
            [Table("shortUrls", Connection = "AzureWebJobsStorage")]out ShortUrl shortUrl)
        {
            //we need to create the short url code
            var shortener = new UrlShortener(CharacterSet.Base58Flickr, initialValue: DateTime.Now.Ticks);


            //we need to output the code to the storage table with the original url
            shortUrl = new ShortUrl
            {
                RowKey = shortener.Current.BaseValue,
                PartitionKey = "mobile-orders",
                TargetUrl = $"https://laundryawayweb.z5.web.core.windows.net/payment-method/method/{order.Id}/{order.CustomerId}/{order.CustomerStripeId}",
            };
            //we need to notify the user via twilio
            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
            var shortUrlVal = $"{Environment.GetEnvironmentVariable("BaseUrl")}{shortener.Current.BaseValue}";

            var message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "payment-method-needed"), new JProperty("shortUrl", shortUrlVal));

            ExecutionResource.Create(
                to: new Twilio.Types.PhoneNumber(order.CustomerId),
                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                parameters: message
            );
        }
    }
}
