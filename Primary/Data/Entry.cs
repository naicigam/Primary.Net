using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

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

    #region JSON serialization

    public class EntryToStringConverter : EnumConverter
    {
        public EntryToStringConverter(Type type) : base(type)
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return StringToEntry[(string)value];
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return EntryToString[(Entry) value];
        }

        private static readonly Dictionary<string, Entry> StringToEntry = new Dictionary<string, Entry>()
        {
            {"BI", Entry.Bids},
            {"OF", Entry.Offers},
        };

        private static readonly Dictionary<Entry, string> EntryToString = new Dictionary<Entry, string>()
        {
            {Entry.Bids, "BI"},
            {Entry.Offers, "OF"},
        };
    }

    #endregion
}
