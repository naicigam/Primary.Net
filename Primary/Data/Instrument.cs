using Newtonsoft.Json;
using Primary.Serialization;
using System;

namespace Primary.Data
{
    /// <summary>Identifies a market instrument.</summary>
    [JsonConverter(typeof(JsonPathDeserializer))]
    public class InstrumentId
    {
        /// <summary>Market where the instruments trades in.</summary>
        /// <remarks>Only accepted value is **ROFX**.</remarks>
        [JsonProperty("marketId")]
        public string Market { get; set; }

        /// <summary>Ticker of the instrument.</summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("instrumentId.marketId")]
        protected string NestedMarket { get { return Market; } set { Market = value; } }

        [JsonProperty("instrumentId.symbol")]
        protected string NestedSymbol { get { return Symbol; } set { Symbol = value; } }

    }

    [JsonConverter(typeof(JsonPathDeserializer))]
    public class Instrument : InstrumentId
    {
        /// <summary>Dezscription of the instrument.</summary>
        [JsonProperty("securityDescription")]
        public string Description { get; set; }

        /// <summary>Currency.</summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>Maturity date (if applicable).</summary>
        [JsonProperty("maturityDate")]
        [JsonConverter(typeof(FlatDateTimeJsonDeserializer))]
        public DateTime? MaturityDate { get; set; }

        /// <summary>Price multiplier to get the price for a single unit.
        /// For example, if an instrument is traded in lots of 100, this value will be 0.01.
        /// </summary>
        [JsonProperty("priceConvertionFactor")]
        public float PriceConvertionFactor { get; set; }
    }
}
