using System;
using System.Collections.Generic;
using System.Text;

namespace Rodzilla.Mobile.Orders.Models
{
    public class OrderQuestion: OrderIdentifier
    {
        public OrderMessage Message { get; set; }
    }
}
