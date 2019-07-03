using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Primary.Serialization;

namespace Primary.Data
{
    [JsonConverter(typeof(EntryJsonSerializer))]
    public enum Entry
    {
        Bids, 
        Offers,
        Last,
        Open,
        Close,
        SettlementPrice,
        SessionHighPrice,
        SessionLowPrice,
        Volume,
        OpenInterest,
        IndexValue,
        EffectiveVolume,
        NominalVolume
    } 

    #region String serialization

    public class EntryJsonSerializer : EnumJsonSerializer<Entry>
    {
        protected override Dictionary<Entry, string> EnumToString =>
            new Dictionary<Entry, string>
            {
                {Entry.Bids, "BI"},
                {Entry.Offers, "OF"},
                {Entry.Last, "LA"},
                {Entry.Open, "OP"},
                {Entry.Close, "CL"},
                {Entry.SettlementPrice, "SE"},
                {Entry.SessionHighPrice, "HI"},
                {Entry.SessionLowPrice, "LO"},
                {Entry.Volume, "TV"},
                {Entry.OpenInterest, "OI"},
                {Entry.IndexValue, "IV"},
                {Entry.EffectiveVolume, "EV"},
                {Entry.NominalVolume, "NV"}
            };
    }

    #endregion
}
