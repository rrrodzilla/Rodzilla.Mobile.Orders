using System;
using Newtonsoft.Json;

namespace Rodzilla.Mobile.Orders.Models
{
    public static class OrderManager
    {
        public static void Ask(string question, ref Order order)
        {
            var newQuestion = new OrderQuestion() { Message = question, Sent = DateTime.Now };
            order.Details.Add(newQuestion);
        }
        public static void Reply(string reply, ref Order order)
        {
            var newReply = new OrderResponse() { Message = reply, Sent = DateTime.Now };
            order.Details.Add(newReply);
        }

        public static string Serialize(Order order)
        {
            return JsonConvert.SerializeObject(order);
        }
        public static Order FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Order>(json);
        }

        public static Order NewOrder(Customer customer)
        {
            return new Order(){CustomerFirstName = customer.FirstName, CustomerId = customer.Id, CustomerStripeId = customer.StripeId};
        }

        public static void EstimateOrder(long price, int minutesToFulfill, ref Order order)
        {
            order.Price = price;
            order.EstimatedMinutes = minutesToFulfill;
        }
    }
}
