namespace Rodzilla.Mobile.Orders.Models
{
    public class PaymentSource
    {
        public string Id { get; set; }
        public string Last4 { get; set; }
        public string Brand { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
    }
}
