﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data
{
    /// <summary>
    /// Market data information requested to the MarketDataWebSocket.
    /// </summary>
    public struct MarketDataInfo
    {
        /// <summary>Order type.</summary>
        [JsonProperty("type", Order=-2)] 
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
        public Instrument[] Products;
    }

    /// <summary>
    /// Real-time market data receive from the MarketDataWebSocket.
    /// </summary>
    public class MarketData
    {
        /// <summary>Instrument id.</summary>
        [JsonProperty("instrumentId")]
        public Instrument Instrument { get; set; }

        /// <summary>Market data grouped by entry.</summary>
        [JsonProperty("marketData")]
        [JsonConverter( typeof( DictionaryJsonSerializer< Entry, IEnumerable<Trade> >) )]
        public Dictionary<Entry, IEnumerable<Trade>> Data { get; set; }
    }
}