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

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("clOrdId")]
        public string ClientOrderId { get; set; }

        [JsonProperty("execId")]
        public string ExecutionId { get; set; }

        [JsonProperty("transactTime")]
        [JsonConverter(typeof(DateTimeJsonDeserializer))]
        public DateTime TransactionTime { get; set; }
        
        [JsonProperty("avgPx")]
        public decimal AveragePrice { get; set; }

        [JsonProperty("lastPx")]
        public decimal LastPrice { get; set; }

        [JsonProperty("lastQty")]
        public decimal LastQuantity { get; set; }

        [JsonProperty("cumQty")]
        public decimal CumulativeQuantity { get; set; }

        [JsonProperty("leavesQty")]
        public uint LeavesQuantity { get; set; }
    }
}
