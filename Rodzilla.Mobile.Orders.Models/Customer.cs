using Newtonsoft.Json;

namespace Rodzilla.Mobile.Orders.Models
{
    [JsonObject]
    public class Customer
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string StripeId { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber => Id;
        public bool IsNew { get; set; }
        public int PointsBalance { get; set; }

        public PaymentSource PaymentSource { get; set; }

        public Customer()
        {
            IsNew = true;
        }
    }
}
