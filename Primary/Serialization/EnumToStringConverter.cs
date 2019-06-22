using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Primary.Serialization
{
    public abstract class EnumToStringConverter<T> : EnumConverter
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
            return EnumToString.FirstOrDefault(x => x.Value == (string)value).Key;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return EnumToString[(T)value];
        }

        protected abstract Dictionary<T, string> EnumToString { get; }
    }
}
