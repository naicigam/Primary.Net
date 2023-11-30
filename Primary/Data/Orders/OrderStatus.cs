using Newtonsoft.Json;
using Primary.Serialization;
using System;

namespace Primary.Data.Orders
{
    /// <summary>
    /// Has all the order information, plus the current state of the order.
    /// </summary>
    public class OrderStatus : Order
    {
        [JsonProperty("orderId")]
        public string Id { get; set; }

        public class AccountId
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        /// <summary>Account of the order.</summary>
        [JsonProperty("accountId")]
        public AccountId Account { get; set; }

        [JsonProperty("execId")]
        public string ExecutionId { get; set; }

        [JsonProperty("transactTime")]
        [JsonConverter(typeof(DateTimeJsonDeserializer))]
        public DateTime TransactionTime { get; set; }

        /// <summary>Average price at which this order was executed.</summary>
        [JsonProperty("avgPx")]
        public decimal AveragePrice { get; set; }

        /// <summary>Last price at which this order was executed.</summary>
        [JsonProperty("lastPx")]
        public decimal LastPrice { get; set; }

        /// <summary>Quantity affected in the last operation.</summary>
        [JsonProperty("lastQty")]
        public decimal LastQuantity { get; set; }

        /// <summary>Total quantity affected on the order.</summary>
        [JsonProperty("cumQty")]
        public decimal CumulativeQuantity { get; set; }

        /// <summary>How much quantity is left on the order.</summary>
        [JsonProperty("leavesQty")]
        public decimal LeavesQuantity { get; set; }

        /// <summary>Order status.</summary>
        [JsonProperty("status")]
        public Status Status { get; set; }

        /// <summary>More information about the order status.</summary>
        [JsonProperty("text")]
        public string StatusText { get; set; }
    }
}
