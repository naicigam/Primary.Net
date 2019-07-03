using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Primary.Serialization
{
    public abstract class EnumJsonSerializer<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueToWrite = EnumToString[(T)value];
            writer.WriteValue(valueToWrite);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return EnumToString.FirstOrDefault(x => x.Value == (string)existingValue).Key;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        
        protected abstract Dictionary<T, string> EnumToString { get; }
    }
}
