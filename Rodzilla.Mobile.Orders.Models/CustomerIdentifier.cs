using System;
using System.Collections.Generic;
using System.Text;

namespace Rodzilla.Mobile.Orders.Models
{
    public class CustomerIdentifier

    {
        public string RowKey { get; set; }
        public string PartitionKey => "";
        public string StripeId { get; set; }
    }
}
