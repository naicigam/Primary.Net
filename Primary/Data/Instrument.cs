using Newtonsoft.Json;

namespace Primary.Data
{
    public class Instrument
    {
        [JsonProperty("marketId")]
        public string Market { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
