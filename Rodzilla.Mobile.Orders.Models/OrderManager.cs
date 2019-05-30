using Newtonsoft.Json;
using System;

namespace Rodzilla.Mobile.Orders.Models
{
    public static class OrderManager
    {
        public static void Ask(OrderQuestion question, ref Order order)
        {
            //var newQuestion = new OrderQuestion() { Message = question, Sent = DateTime.Now };
            order.Details.Add(question.Message);
        }
        public static void Reply(string reply, ref Order order)
        {
            var seconds = (long)DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            var newReply = new OrderMessage() { Message = reply, Sent = DateTime.Now, Sender = order.Customer.FirstName, SentSeconds = seconds };
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
            return new Order() { Customer = customer };
        }

        public static void EstimateOrder(long price, int minutesToFulfill, ref Order order)
        {
            order.Price = price;
            order.EstimatedMinutes = minutesToFulfill;
        }
    }
}
