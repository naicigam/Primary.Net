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
        public string ClientOrderId { get; set; }

        /// <summary>
        /// Identifies the order sent by web socket.
        /// This will only be set the first time order information is received by web socket.
        /// To identify the order after, use ClientOrderId.
        /// </summary>
        [JsonProperty("wsClOrdId")]
        public string WebSocketClientOrderId { get; set; }
    }
}
