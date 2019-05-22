using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;
using Stripe;
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
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            var options = new CustomerUpdateOptions
            {                
                SourceToken = orderTokenIdentifier.PaymentToken
            };

            var service = new CustomerService();
            var stripeCustomer = await service.UpdateAsync(orderTokenIdentifier.CustomerStripeId, options);
            //let's cache this source info locally
            var card = ((Card) (stripeCustomer.Sources.Data[0]));
            customer.PaymentSource = new PaymentSource() { Brand = card.Brand, Last4 = card.Last4, ExpMonth = card.ExpMonth, ExpYear = card.ExpYear, Id = card.Id};
            order.OrderStatus = "estimate-accepted";        
        }
    }
}
