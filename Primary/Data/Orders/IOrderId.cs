using Newtonsoft.Json;

namespace Primary.Data.Orders
{
    internal interface IOrderId
    {
        [JsonProperty("clientId")]
        ulong ClientId { get; set; }

        [JsonProperty("proprietary")]
        string Proprietary { get; set; }
    }

    public class OrderId : IOrderId
    {
        public ulong ClientId { get; set; }
        public string Proprietary { get; set; }
    }
}
