using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
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
                collectionName: "orders", ConnectionStringSetting = "DBConnection", Id="{OrderId}", PartitionKey = "{CustomerId}")]Order order)
        {
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            var options = new CustomerUpdateOptions
            {                
                SourceToken = orderTokenIdentifier.PaymentToken
            };

            var service = new CustomerService();
            await service.UpdateAsync(orderTokenIdentifier.CustomerStripeId, options);

            order.OrderStatus = "estimate-accepted";        
        }
    }
}
