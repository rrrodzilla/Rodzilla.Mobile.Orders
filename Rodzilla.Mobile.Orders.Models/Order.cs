﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rodzilla.Mobile.Orders.Models
{
    [JsonObject]
    public class Order
    {

        public string OrderStatus { get; set; }
        public string CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerStripeId { get; set; }
        public long Price { get; set; }
        public string DisplayPrice => $"{((decimal) QuotedPrice / 100):C}";
        public long AppFee => (long) (Price * .15);
        public long QuotedPrice => Price + AppFee;
        public int EstimatedMinutes { get; set; }
        public int PointsValue => (int) (QuotedPrice / 100)*2;
        public int PointsPrice { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public int? MinutesToComplete => (FinishTime - StartTime).Minutes;

        public OrderMessage OriginalOrder { get; set; }

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<OrderMessage> Details { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }

        public Order()
        {
            Id = Guid.NewGuid().ToString();
            Details = new List<OrderMessage>();
            OrderStatus = "requested";
        }
    }


}