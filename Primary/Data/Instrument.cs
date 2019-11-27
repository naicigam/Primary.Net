using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>Identifies a market instrument.</summary>
    public class Instrument
    {
        /// <summary>Market where the instruments belongs to.</summary>
        /// <remarks>Only accepted value is **ROFX**.</remarks>
        [JsonProperty("marketId")]
        public string Market { get; set; }

        /// <summary>Ticker of the instrument.</summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
