using Newtonsoft.Json;

namespace Primary.Data.Orders
{
    public class OrderId
    {
        [JsonProperty("clientId")]
        public ulong ClientId { get; set; }
        
        [JsonProperty("proprietary")]
        public string Proprietary { get; set; }

        [JsonProperty("orderId")]
        public string Id { get; set; }

        [JsonProperty("clOrdId")]
        public ulong ClientOrderId { get; set; }
    }
}
