using Newtonsoft.Json;
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

    #region String serialization

    internal static class EnumsToApiStrings
    {
        public static string ToApiString(this Entry value)
        {
            switch (value)
            {
                case Entry.Bids: return "BI";
                case Entry.Offers: return "OF";
                case Entry.Last: return "LA";
                case Entry.Open: return "OP";
                case Entry.Close: return "CL";
                case Entry.SettlementPrice: return "SE";
                case Entry.SessionHighPrice: return "HI";
                case Entry.SessionLowPrice: return "LO";
                case Entry.Volume: return "TV";
                case Entry.OpenInterest: return "OI";
                case Entry.IndexValue: return "IV";
                case Entry.EffectiveVolume: return "EV";
                case Entry.NominalVolume: return "NV";
                default: throw new InvalidEnumStringException( value.ToString() );
            }
        }

        public static Entry EntryFromApiString(string value)
        {
            switch (value)
            {
                case "BI": return Entry.Bids;
                case "OF": return Entry.Offers;
                case "LA": return Entry.Last;
                case "OP": return Entry.Open;
                case "CL": return Entry.Close;
                case "SE": return Entry.SettlementPrice;
                case "HI": return Entry.SessionHighPrice;
                case "LO": return Entry.SessionLowPrice;
                case "TV": return Entry.Volume;
                case "OI": return Entry.OpenInterest;
                case "IV": return Entry.IndexValue;
                case "EV": return Entry.EffectiveVolume;
                case "NV": return Entry.NominalVolume;
                default: throw new InvalidEnumStringException(value);
            }
        }
    }

    #endregion

    #region JSON Serialization

    internal class EntryJsonSerializer : EnumJsonSerializer<Entry>
    {
        protected override string ToString(Entry enumValue)
        {
            return enumValue.ToApiString();
        }

        protected override Entry FromString(string enumString)
        {
            return EnumsToApiStrings.EntryFromApiString(enumString);
        }
    }

    #endregion
}
