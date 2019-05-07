using System;
using System.Collections.Generic;
using System.Text;

namespace Rodzilla.Mobile.Orders.Models
{
    public class OrderEstimate : OrderIdentifier
    {
        public int EstimatedMinutes { get; set; }
        public long Price { get; set; }
    }
}
