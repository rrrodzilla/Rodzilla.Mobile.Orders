using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;
using Stripe;
using Customer = Rodzilla.Mobile.Orders.Models.Customer;

namespace Rodzilla.Mobile.Orders
{
    public static class FetchCustomerByPhone
    {

        [FunctionName("FetchCustomerByPhone")]
        [return: Queue("get-customer", Connection = "AzureWebJobsStorage")]
        public static async Task<CustomerIdentifier> Run([QueueTrigger("get-customer-by-phone", Connection = "AzureWebJobsStorage")]Customer incomingCustomer, ILogger log,
            [Table("customers", "", "{Id}", Connection = "AzureWebJobsStorage")]CustomerIdentifier customer)
        {
            StripeConfiguration.SetApiKey(Environment.GetEnvironmentVariable("StripeKey"));

            if (customer == null)
            {
                //if there is no customer in the table storage, let's create it
                customer = new CustomerIdentifier() { RowKey = incomingCustomer.Id };
            }

            //does customer have a stripe id?
            if (customer.StripeId == null)
            {
                //if not create one
                var stripeCustomer = await CreateStripeCustomer(incomingCustomer.PhoneNumber);
                customer.StripeId = stripeCustomer.Id;
            }

            log.LogInformation($"Stripe ID: {customer.StripeId}");
            return customer;

            async Task<Stripe.Customer> CreateStripeCustomer(string phoneNumber)
            {
                var options = new CustomerCreateOptions
                {
                    Description = $"Customer for {phoneNumber}",
                    Phone = phoneNumber
                };

                var service = new CustomerService();
                var stripeCustomer = await service.CreateAsync(options);
                
                return stripeCustomer;
            }
        }
    }
}
