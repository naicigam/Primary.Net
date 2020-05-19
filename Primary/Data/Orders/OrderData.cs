using System;
using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data.Orders
{
    public class OrderData : Order
    {
        public class AccountId
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        [JsonProperty("accountId")]
        public AccountId Account { get; set; }

        [JsonProperty("execId")]
        public string ExecutionId { get; set; }

        [JsonProperty("transactTime")]
        [JsonConverter(typeof(DateTimeJsonDeserializer))]
        public DateTime TransactionTime { get; set; }
        
        [JsonProperty("avgPx")]
        public decimal AveragePrice { get; set; }

        [JsonProperty("lastPx")]
        public decimal LastPrice { get; set; }

        /// <summary>Quantity affected in the last operation.</summary>
        [JsonProperty("lastQty")]
        public uint LastQuantity { get; set; }

        /// <summary>Total quantity affected on the order.</summary>
        [JsonProperty("cumQty")]
        public uint CumulativeQuantity { get; set; }

        /// <summary>How much quantity is left on the order.</summary>
        [JsonProperty("leavesQty")]
        public uint LeavesQuantity { get; set; }
    }
}
