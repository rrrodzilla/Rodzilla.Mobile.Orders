namespace Rodzilla.Mobile.Orders.Models
{
    public class OrderTokenIdentifier 
    {
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
        public string PaymentToken { get; set; }
        public string Last4 { get; set; }
        public string Brand { get; set; }
        public string CustomerStripeId { get; set; }
    }
}
