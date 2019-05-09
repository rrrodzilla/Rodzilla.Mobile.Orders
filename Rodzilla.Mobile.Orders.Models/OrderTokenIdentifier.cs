using System;
using System.Collections.Generic;
using System.Text;

namespace Rodzilla.Mobile.Orders.Models
{
    public class OrderTokenIdentifier : OrderIdentifier
    {
        public string PaymentToken { get; set; }
        public string CustomerStripeId { get; set; }
    }
}
