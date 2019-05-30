using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;
using Stripe;
using System;
using System.Threading.Tasks;
using Customer = Rodzilla.Mobile.Orders.Models.Customer;
using Order = Rodzilla.Mobile.Orders.Models.Order;

namespace Rodzilla.Mobile.Orders
{
    public static class IncomingPaymentToken
    {
        [FunctionName("IncomingPaymentToken")]
        public static async Task Run([QueueTrigger("receive-new-payment-token", Connection = "AzureWebJobsStorage")]OrderTokenIdentifier orderTokenIdentifier, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers", ConnectionStringSetting = "DBConnection", Id="{CustomerId}", PartitionKey = "{CustomerStripeId}")]Customer customer)
        {
            //first we want to check if this order has already been paid
            if (order == null || order.OrderStatus == "order-fulfilled")
            {
                log.LogInformation("Order has already been fulfilled");
                return ;
            }
            if (customer?.StripeId == null)
            {
                log.LogInformation("Customer.StripeId didn't exist");
                return ;
            }

            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            var options = new CustomerUpdateOptions
            {
                SourceToken = orderTokenIdentifier.PaymentToken
            };

            var service = new CustomerService();
            var stripeCustomer = await service.UpdateAsync(orderTokenIdentifier.CustomerStripeId, options);
            //let's cache this source info locally
            var source = ((Source)(stripeCustomer.Sources.Data[0]));
            var card = source.Card;
            var newCard = new PaymentSource() { Brand = card.Brand, Last4 = card.Last4, ExpMonth = card.ExpMonth, ExpYear = card.ExpYear, Id = source.Id };

            customer.PaymentSource = newCard;

            order.Customer.PaymentSource = newCard;

            //not sure why i did this.. maybe for testing?  commenting out for now
            order.OrderStatus = "estimate-accepted";
        }
    }
}
