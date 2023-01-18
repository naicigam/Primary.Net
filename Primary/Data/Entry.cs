﻿using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data
{
    [JsonConverter(typeof(EntryJsonSerializer))]
    public enum Entry
    {
        /// <summary>Best buy offer in the Market Book.</summary>
        Bids,

        /// <summary>Best sell offer in the Market Book.</summary>
        Offers,

        /// <summary>Last price traded in the Market Book.</summary>
        Last,

        /// <summary>Opening price in the Market Book.</summary>
        Open,

        /// <summary>Closing price in the Market Book.</summary>
        Close,

        /// <summary>Settlement price (only for futures).</summary>
        SettlementPrice,

        /// <summary>Highest price traded.</summary>
        SessionHighPrice,

        /// <summary>Lowest price traded.</summary>
        SessionLowPrice,

        /// <summary>Traded volume in contracts/nominal.</summary>
        Volume,

        /// <summary>Open interest in contracts (only for futures).</summary>
        OpenInterest,

        /// <summary>Calculated index value (only for indices).</summary>
        IndexValue,

        /// <summary>Effective traded volume.</summary>
        EffectiveVolume,

        /// <summary>Nominal traded volume.</summary>
        NominalVolume
    }

    #region JSON Serialization

    internal class EntryJsonSerializer : EnumJsonSerializer<Entry>
    {
        protected override string ToString(Entry enumValue)
        {
            return enumValue switch
            {
                Entry.Bids => "BI",
                Entry.Offers => "OF",
                Entry.Last => "LA",
                Entry.Open => "OP",
                Entry.Close => "CL",
                Entry.SettlementPrice => "SE",
                Entry.SessionHighPrice => "HI",
                Entry.SessionLowPrice => "LO",
                Entry.Volume => "TV",
                Entry.OpenInterest => "OI",
                Entry.IndexValue => "IV",
                Entry.EffectiveVolume => "EV",
                Entry.NominalVolume => "NV",
                _ => throw new InvalidEnumStringException(enumValue.ToString())
            };
        }

        protected override Entry FromString(string enumString)
        {
            return enumString switch
            {
                "BI" => Entry.Bids,
                "OF" => Entry.Offers,
                "LA" => Entry.Last,
                "OP" => Entry.Open,
                "CL" => Entry.Close,
                "SE" => Entry.SettlementPrice,
                "HI" => Entry.SessionHighPrice,
                "LO" => Entry.SessionLowPrice,
                "TV" => Entry.Volume,
                "OI" => Entry.OpenInterest,
                "IV" => Entry.IndexValue,
                "EV" => Entry.EffectiveVolume,
                "NV" => Entry.NominalVolume,
                _ => throw new InvalidEnumStringException(enumString)
            };
        }
    }

    #endregion
}
