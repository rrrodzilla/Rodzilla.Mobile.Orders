using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Rodzilla.Mobile.Orders.Models;
using System;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class SendReceipt
    {
        [FunctionName("SendReceipt")]
        public static void Run([QueueTrigger("mo-send-receipt", Connection = "AzureWebJobsStorage")]Order incomingOrder, ILogger log,
            [Table("shortUrls", Connection = "AzureWebJobsStorage")]out ShortUrl shortUrl)
        {

            //we need to create the short url code
            var shortener = new UrlShortener(CharacterSet.Base58Flickr, initialValue: DateTime.Now.Ticks);


            //we need to output the code to the storage table with the original url
            shortUrl = new ShortUrl
            {
                RowKey = shortener.Current.BaseValue,
                PartitionKey = "mobile-orders",
                TargetUrl = incomingOrder.ReceiptUrl,
            };
            //we need to notify the user via twilio

            incomingOrder.ReceiptUrl = $"{Environment.GetEnvironmentVariable("BaseUrl")}{shortener.Current.BaseValue}";


            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
            var message = new JObject(new JProperty("order", JObject.FromObject(incomingOrder)), new JProperty("action", "order-paid"));

            ExecutionResource.Create(
                to: new Twilio.Types.PhoneNumber(incomingOrder.CustomerId),
                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                parameters: message
            );

        }
    }
}
