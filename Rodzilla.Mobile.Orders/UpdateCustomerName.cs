using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Stripe;
using Customer = Rodzilla.Mobile.Orders.Models.Customer;

namespace Rodzilla.Mobile.Orders
{
    public static class UpdateCustomerName
    {
        [FunctionName("UpdateCustomerName")]
        [return: Queue("begin-order", Connection = "AzureWebJobsStorage")]
        public static async Task<Customer> Run([QueueTrigger("customer-update-name", Connection = "AzureWebJobsStorage")]Customer incomingCustomer, ILogger log,
            [CosmosDB(
                databaseName: "MobileOrders",
                collectionName: "customers", ConnectionStringSetting = "DBConnection", Id="{Id}", PartitionKey = "{StripeId}")]Customer customer)
        {
            //first we want to update the customer in stripe
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));
            var options = new CustomerUpdateOptions
            {
                Name = incomingCustomer.FirstName,
                Email = incomingCustomer.Email
            };

            var service = new CustomerService();
            await service.UpdateAsync(customer.StripeId, options);


            //then update the customer in the db
            customer.FirstName = incomingCustomer.FirstName;
            customer.Email = incomingCustomer.Email;
            customer.IsNew = false;

            return customer;
        }
    }
}
