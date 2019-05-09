using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stripe;
using Customer = Rodzilla.Mobile.Orders.Models.Customer;
using Order = Rodzilla.Mobile.Orders.Models.Order;

namespace Rodzilla.Mobile.Orders
{
    public static class PayOrder
    {
        [FunctionName("PayOrder")]
        [return: Queue("obtain-new-payment-method", Connection = "AzureWebJobsStorage")]
        public static async Task<Order> Run([QueueTrigger("pay-order-requests", Connection = "AzureWebJobsStorage")]Order payRequestOrder, 
            ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{Id}", PartitionKey = "{CustomerId}")]Order order,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers", ConnectionStringSetting = "DBConnection", Id="{CustomerId}", PartitionKey = "{CustomerStripeId}")]Customer customer)
        {
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            log.LogInformation("\n\n\n************ PAYING ORDER WITH STRIPE *************\n\n\n");
            //here we want to know if they have a payment method
            var service = new PaymentMethodService();
            var options = new PaymentMethodListOptions
            {
                Limit = 1,
                CustomerId = customer.StripeId,
                Type = "card"
            };
            var paymentMethods = service.List(options);

            if (paymentMethods.Data.Count == 0)
            {
                //we need to return a message to a queue to go get a payment method for this user
                order.OrderStatus = "payment-method-needed";
                return order;
            }

            var chargeOptions = new ChargeCreateOptions
            {
                Amount = order.QuotedPrice,
                Currency = "usd",
                Description = $"Jibe Espresso Bar - Mobile Order including {order.DisplayAppFee} platform fee",
                CustomerId = order.CustomerStripeId
            };
            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(chargeOptions);

            order.TransactionId = charge.Id;
            order.ReceiptUrl = charge.ReceiptUrl;
            order.OrderStatus = "ready-for-fulfillment";

            log.LogInformation("\n\n\n************ Updating Points Balance *************\n\n\n");
            customer.PointsBalance += order.PointsValue;
            return null;
        }
    }
}
