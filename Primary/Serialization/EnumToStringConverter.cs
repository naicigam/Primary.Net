using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Primary.Data;

namespace Primary.Serialization
{
    public class EnumToStringConverter<T> : EnumConverter
    {
        protected EnumToStringConverter() : base(typeof(T))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return _enumToString.FirstOrDefault(x => x.Value == (string)value).Key;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return _enumToString[(T)value];
        }

        protected Dictionary<T, string> _enumToString;
    }
}
