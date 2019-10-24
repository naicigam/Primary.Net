using Newtonsoft.Json;

namespace Primary.Data.Orders
{
    public class OrderId
    {
        [JsonProperty("clientId")]
        public ulong ClientId { get; set; }
        
        [JsonProperty("proprietary")]
        public string Proprietary { get; set; }
    }
}
