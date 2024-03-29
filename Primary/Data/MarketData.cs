﻿using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>
    /// Market data information requested to the MarketDataWebSocket.
    /// </summary>
    public struct MarketDataInfo
    {
        /// <summary>Order type.</summary>
        [JsonProperty("type", Order = -2)]
        public string Type => "smd";

        /// <summary>
        /// Real-time message update time.
        /// <list type="table">
        ///     <listheader> <term>Level</term> <description>Update time (ms)</description> </listheader>
        ///     <item> <term>1</term> <description>100</description> </item>
        ///     <item> <term>2</term> <description>500</description> </item>
        ///     <item> <term>3</term> <description>1000</description> </item>
        ///     <item> <term>4</term> <description>3000</description> </item>
        ///     <item> <term>5</term> <description>6000</description> </item>
        /// </list>
        /// </summary>
        [JsonProperty("level")]
        public uint Level;

        /// <summary>Book depth.</summary>
        /// <value>Default: 1</value>
        [JsonProperty("depth")]
        public uint Depth;

        /// <summary>Requested entries.</summary>
        [JsonProperty("entries")]
        public Entry[] Entries;

        /// <summary>Products to get the information for.</summary>
        [JsonProperty("products")]
        public InstrumentId[] Products;
    }

    /// <summary>
    /// Real-time market data receive from the MarketDataWebSocket.
    /// </summary>
    public class MarketData
    {
        /// <summary>Server time in epoch format.</summary>
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>Instrument id.</summary>
        [JsonProperty("instrumentId")]
        public InstrumentId InstrumentId { get; set; }

        /// <summary>Market data grouped by entry.</summary>
        [JsonProperty("marketData")]
        public Entries Data { get; set; }

        public class Entries
        {
            [JsonProperty("BI")] public PriceSize[] Bids { get; set; }
            [JsonProperty("OF")] public PriceSize[] Offers { get; set; }

            [JsonProperty("LA")] public PriceDate Last { get; set; }
            [JsonProperty("OP")] public decimal? Open { get; set; }
            [JsonProperty("CL")] public PriceDate Close { get; set; }

            [JsonProperty("SE")] public PriceDate SettlementPrice { get; set; }
            [JsonProperty("OI")] public SizeDate OpenInterest { get; set; }

            [JsonProperty("HI")] public decimal? SessionHighPrice { get; set; }
            [JsonProperty("LO")] public decimal? SessionLowPrice { get; set; }
            [JsonProperty("IV")] public decimal? IndexValue { get; set; }

            [JsonProperty("TV")] public decimal? Volume { get; set; }
            [JsonProperty("NV")] public decimal? NominalVolume { get; set; }
            [JsonProperty("EV")] public decimal? EffectiveVolume { get; set; }
        }
    }
}
