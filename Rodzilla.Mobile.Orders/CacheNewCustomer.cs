using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rodzilla.Mobile.Orders.Models;
using System.Collections.Generic;

namespace Rodzilla.Mobile.Orders
{
    public static class CacheNewCustomer
    {
        [FunctionName("CacheNewCustomer")]
        public static void Run([CosmosDBTrigger(
            databaseName: "MobileOrders",
            collectionName: "customers",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "new-customer-leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log,
            [Table("customers", Connection = "AzureWebJobsStorage")] ICollector<CustomerIdentifier> cachedCustomers)
        {
            if (input == null || input.Count <= 0) return;
            foreach (var document in input)
            {
                var customer = CustomerManager.FromJson(document.ToString());
                if (customer.IsNew)
                {
                    cachedCustomers?.Add(new CustomerIdentifier()
                        { RowKey = customer.PhoneNumber, StripeId = customer.StripeId });
                }
            }
        }
    }
}
