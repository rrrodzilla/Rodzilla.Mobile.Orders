using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Rodzilla.Mobile.Orders.Models
{
    public static class CustomerManager
    {
        public static string Serialize(Customer customer)
        {
            return JsonConvert.SerializeObject(customer);
        }
        public static Customer FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Customer>(json);
        }
    }
}
