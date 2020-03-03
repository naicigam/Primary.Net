using System.Collections.Generic;
using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data
{
    public struct MarketDataInfo
    {
        [JsonProperty("type", Order=-2)] 
        public string Type => "smd";
            
        [JsonProperty("level")]
        public uint Level;

        [JsonProperty("depth")]
        public uint Depth;

        [JsonProperty("entries")]
        public Entry[] Entries;

        [JsonProperty("products")]
        public Instrument[] Products;
    }

    public class MarketData
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonProperty("instrumentId")]
        public Instrument Instrument { get; set; }

        [JsonProperty("marketData")]
        [JsonConverter( typeof( DictionaryJsonSerializer< Entry, IEnumerable<Trade> >) )]
        public Dictionary<Entry, IEnumerable<Trade>> Data { get; set; }
    }
}
