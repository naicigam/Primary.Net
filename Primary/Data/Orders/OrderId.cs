using Newtonsoft.Json;

namespace Primary.Data.Orders
{
    /// <summary>
    /// Identifies an order.
    /// </summary>
    public class OrderId
    {        
        [JsonProperty("proprietary")]
        public string Proprietary { get; set; }

        [JsonProperty("clOrdId")]
        public ulong ClientOrderId { get; set; }
    }
}
