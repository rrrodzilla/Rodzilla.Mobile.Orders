using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Studio.V1.Flow;

namespace Rodzilla.Mobile.Orders
{
    public static class Notify
    {
        [FunctionName("Notify")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "MobileOrders",
            collectionName: "orders",
            ConnectionStringSetting = "DbConnection",
            LeaseCollectionName = "leases-notify-order-started", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log,
            [SignalR(HubName = "orders")]IAsyncCollector<SignalRMessage> signalRMessages
            )

        {
            if (input != null && input.Count > 0)
            {
                foreach (var document in input)
                {
                    var order = OrderManager.FromJson(document.ToString());
                    log.LogInformation("\n\n\n");
                    JObject message;
                    switch (order.OrderStatus)
                    {
                        case "estimate-accepted":
                            log.LogInformation($"\n\n\n************************* ORDER ESTIMATE ACCEPTED *********************\n\n\n");
                            break;

                        case "order-fulfilled":
                            log.LogInformation(
                                $"\n\n\n********************** ORDER FINISHED AT {order.FinishTime} ********************\n{order.CustomerFirstName}, your order is ready for pickup!\nSkip the line and give your name at our counter.  \nIt took {order.MinutesToComplete} minutes to prepare and you earned {order.PointsValue} reward points.\n******************************************\n\n\n");
                            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                            message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "order-fulfilled"));

                            await ExecutionResource.CreateAsync(
                                to: new Twilio.Types.PhoneNumber(order.CustomerId),
                                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                                parameters: message
                            );
                            break;

                        case "order-started":
                            log.LogInformation(
                                $"\n\n\n********************** ORDER STARTED AT {order.StartTime} ********************\n{order.CustomerFirstName}, we've started your order at Jibe Espresso Bar.  We'll text you when its ready for pickup!\n******************************************\n\n\n");
                            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                            message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "order-started"));

                            await ExecutionResource.CreateAsync(
                                to: new Twilio.Types.PhoneNumber(order.CustomerId),
                                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                                parameters: message
                            );
                            break;

                        case "ready-for-fulfillment":
                            log.LogInformation($"\n\n\n*********** ORDER READY TO BE FULFILLED ***************\nOrder Id: {order.Id} for Customer Id: {order.CustomerId} is ready to be made!\n******************************************\n\n\n");
                            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                            message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "order-paid"));

                            //await ExecutionResource.CreateAsync(
                            //    to: new Twilio.Types.PhoneNumber(order.CustomerId),
                            //    from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                            //    pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                            //    parameters: message
                            //);
                            await signalRMessages.AddAsync(
                                new SignalRMessage
                                {
                                    Target = "updateStatus",
                                    Arguments = new object[] { order }
                                });
                            break;
                        case "estimated":
                            log.LogInformation($"\n\n\n************************ ESTIMATE ALERT ***************************\nThank you for your order, {order.CustomerFirstName}.\nWe look forward to serving you.\nWe can have your order ready in about {order.EstimatedMinutes} minutes.\nThe total price including taxes and application fees is {((decimal)order.QuotedPrice / 100):C}.\nWould you like to place your order now?\n***************************************************\n\n\n");
                            TwilioClient.Init(Environment.GetEnvironmentVariable("Twilio_Account_Sid"), Environment.GetEnvironmentVariable("Twilio_Auth_Token"));
                            message = new JObject(new JProperty("order", JObject.FromObject(order)), new JProperty("action", "received-estimate"));

                            await ExecutionResource.CreateAsync(
                                to: new Twilio.Types.PhoneNumber(order.CustomerId),
                                from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("Twilio_Phone_Number")),
                                pathFlowSid: Environment.GetEnvironmentVariable("Twilio_Flow_Id"),
                                parameters: message
                            );
                            break;
                        case "requested":
                            log.LogInformation($"\n\n\n*********** NEW ORDER REQUEST *******************\n{order.CustomerFirstName} has placed the following order at {order.OriginalOrder.Sent}:\nRequest: {order.OriginalOrder.Message}\n***************************************************\n\n\n");
                            break;

                    }
                    log.LogInformation("\n\n\n");
                }
            }
        }
    }
}
