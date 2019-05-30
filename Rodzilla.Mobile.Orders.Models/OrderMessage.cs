using Newtonsoft.Json;
using System;

namespace Rodzilla.Mobile.Orders.Models
{
    [JsonObject]
    public class OrderMessage
    {
        public string Message { get; set; }
        public DateTime Sent { get; set; }
        public string Sender { get; set; }
        public long SentSeconds { get; set; }

        public OrderMessage()
        {
            Sent = DateTime.Now;
        }
    }



    //[JsonObject(Title = "OrderQuestion")]
    //public class OrderQuestion : OrderMessage { }
    //[JsonObject(Title = "OrderResponse")]
    //public class OrderResponse : OrderMessage { }

}
