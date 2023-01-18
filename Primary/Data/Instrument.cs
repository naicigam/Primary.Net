using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        #region JSON serialization

        [JsonProperty("instrumentId.marketId")]
        protected string NestedMarket { get { return Market; } set { Market = value; } }

        [JsonProperty("instrumentId.symbol")]
        protected string NestedSymbol { get { return Symbol; } set { Symbol = value; } }

        /// <summary>This is used for serialization purposes and should not be called.</summary>
        public bool ShouldSerializeNestedMarket() { return false; }

        /// <summary>This is used for serialization purposes and should not be called.</summary>
        public bool ShouldSerializeNestedSymbol() { return false; }

        #endregion
    }

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
        public float PriceConversionFactor { get; set; }

        /// <summary>Instrument type according to <seealso href="https://www.iso.org/standard/81140.html">ISO 10962</seealso>.</summary>
        [JsonProperty("cficode")]
        public string CfiCode { get; set; }

        /// <summary>Instrument type from CFI code.</summary>
        public InstrumentType Type
        {
            get
            {
                return CfiCode switch
                {
                    "ESXXXX" => InstrumentType.Equity,
                    "DBXXXX" => InstrumentType.Bond,
                    "OCASPS" => InstrumentType.EquityCallOption,
                    "OPASPS" => InstrumentType.EquityPutOption,
                    "FXXXSX" => InstrumentType.Future,
                    "OPAFXS" => InstrumentType.FuturePutOption,
                    "OCAFXS" => InstrumentType.FutureCallOption,
                    "EMXXXX" => InstrumentType.Cedear,
                    "DBXXFR" => InstrumentType.Obligation,
                    "MRIXXX" => InstrumentType.Index,
                    "FXXXXX" => InstrumentType.Future,
                    "RPXXXX" => InstrumentType.Caucion,
                    "MXXXXX" => InstrumentType.Miscellaneous,
                    "LRSTXH" => InstrumentType.Miscellaneous,
                    "DYXTXR" => InstrumentType.TreasureNotes,
                    _ => InstrumentType.Unknown
                };
            }
        }
    }

    public enum InstrumentType
    {
        Unknown,
        Equity,
        Future,
        Option,
        Bond,
        Cedear,
        Obligation,
        EquityCallOption,
        EquityPutOption,
        FutureCallOption,
        FuturePutOption,
        Caucion,
        Index,
        TreasureNotes,
        Miscellaneous
    }
}
