using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Primary.Data
{
    public class Trade
    {
        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("size")]
        public float Size { get; set; }

        [JsonProperty("datetime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("servertime")]
        public long ServerTime { get; set; }
    }

    public class MarketData
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonProperty("instrumentId")]
        public Instrument Instrument { get; set; }

        [JsonProperty("marketData")]
        public Dictionary<Entry, IEnumerable<Trade>> Data { get; set; }
    }
}
