using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Threading.Tasks;
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
                collectionName: "customers", ConnectionStringSetting = "DBConnection", Id="{CustomerId}", PartitionKey = "{CustomerStripeId}")]Customer customer,
            [Queue("mo-send-receipt", Connection = "AzureWebJobsStorage")]IAsyncCollector<Order> receipts, 
            [Queue("mo-payment-method-url", Connection = "AzureWebJobsStorage")]IAsyncCollector<Order> paymentMethod)
        {
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            log.LogInformation("\n\n\n************ PAYING ORDER WITH STRIPE *************\n\n\n");
            //first we want to check if this order has already been paid
            if (order == null || order.OrderStatus == "order-fulfilled") return null;
            if (customer?.StripeId == null) return null;

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
                //if not, we need to return a message to a queue to go get a payment method for this user
                await paymentMethod.AddAsync(order);
                order.OrderStatus = "payment-method-needed";
                return order;
            }

            //if so, let's complete the payment
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = order.QuotedPrice,
                Currency = "usd",
                Description = $"Jibe Espresso Bar\n{order.OriginalOrder.Message}\nIncludes {order.DisplayAppFee} mobile order convenience fee",
                CustomerId = order.CustomerStripeId
            };
            var chargeService = new ChargeService();
            var charge = await chargeService.CreateAsync(chargeOptions);

            order.TransactionId = charge.Id;
            order.TransactionTime = DateTime.Now;
            order.ReceiptUrl = charge.ReceiptUrl;
            order.OrderStatus = "ready-for-fulfillment";


            log.LogInformation("\n\n\n************ Updating Points Balance *************\n\n\n");
            customer.PointsBalance += order.PointsValue;
            order.PostPointsBalance = customer.PointsBalance;
            await receipts.AddAsync(order);
            return null;
        }
    }
}
