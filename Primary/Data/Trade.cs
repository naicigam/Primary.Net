using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>
    /// Contains the information about a trade.
    /// </summary>
    public class Trade : Date, ISize, IPrice
    {
        public decimal Price { get; set; }
        public decimal Size { get; set; }

        [JsonProperty("servertime")]
        public long ServerTime { get; set; }
    }
}
