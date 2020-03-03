using System;
using Newtonsoft.Json;

namespace Primary.Data
{
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
