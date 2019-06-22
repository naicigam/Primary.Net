using System.Collections.Generic;
using System.ComponentModel;
using Primary.Serialization;

namespace Primary.Data
{
    [TypeConverter(typeof(EntryToStringConverter))]
    public class Entry : ValueType
    {
        public static Entry Bids = new Entry("Bids");
        public static Entry Offers = new Entry("Offers");
        //public static Entry Last,
        //public static Entry Open,
        //public static Entry Close,
        //public static Entry SettlementPrice,
        //public static Entry SessionHighPrice,
        //public static Entry SessionLowPrice,
        //public static Entry Volume,
        //public static Entry OpenInterest,
        //public static Entry IndexValue,
        //public static Entry EffectiveVolume,
        //public static Entry NominalVolume

        public Entry(string value) : base(value)
        {
        }
    } 

    #region String serialization

    public class EntryToStringConverter : EnumToStringConverter<Entry>
    {
        protected override Dictionary<Entry, string> EnumToString =>
            new Dictionary<Entry, string>
            {
                {Entry.Bids, "BI"},
                {Entry.Offers, "OF"}
            };
    }

    #endregion
}
