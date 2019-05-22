using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Rodzilla.Mobile.Orders.Models
{
    [JsonObject]
    public class ShortUrl : TableEntity
    {
        public new string RowKey { get; set; }
        public new string PartitionKey { get; set; }
        public string TargetUrl { get; set; }
    }
}
