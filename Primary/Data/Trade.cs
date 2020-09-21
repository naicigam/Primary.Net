using System;
using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>
    /// Contains the information about a trade.
    /// </summary>
    public class Trade
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("size")]
        public decimal Size { get; set; }

        [JsonProperty("datetime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("servertime")]
        public long ServerTime { get; set; }
    }
}
