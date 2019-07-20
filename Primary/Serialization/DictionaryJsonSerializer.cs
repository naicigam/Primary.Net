using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Primary.Serialization
{
    // Required to serialize dictionary keys: https://github.com/JamesNK/Newtonsoft.Json/issues/1371
    // TODO: Find another option.
    internal class DictionaryJsonSerializer<TKey, TValue> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var canCovert = objectType.FullName == typeof(Dictionary<TKey, TValue>).FullName;
            return canCovert;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var (keyProperty, valueProperty) = GetKeyAndValueProperties(value);
            var keys = (IEnumerable)keyProperty.GetValue(value, null);
            var values = ((IEnumerable)valueProperty.GetValue(value, null));
            var valueEnumerator = values.GetEnumerator();

            writer.WriteStartArray();

            foreach (var eachKey in keys)
            {
                valueEnumerator.MoveNext();

                writer.WriteStartArray();

                serializer.Serialize(writer, eachKey);
                serializer.Serialize(writer, valueEnumerator.Current);

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        private static (PropertyInfo, PropertyInfo) GetKeyAndValueProperties(object value)
        {
            var type = value.GetType();
            var keyProperty = type.GetProperty("Keys");

            if (keyProperty == null)
            {
                throw new InvalidOperationException($"{value.GetType().Name} was expected to be a {typeof(Dictionary<TKey, TValue>).Name}, and doesn't have a Keys property.");
            }

            var valueProperty = type.GetProperty("Values");

            if (valueProperty == null)
            {
                throw new InvalidOperationException($"{value.GetType().Name} was expected to be a {typeof(Dictionary<TKey, TValue>).Name}, and doesn't have a Values property.");
            }

            return (keyProperty, valueProperty);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.FullName != typeof(Dictionary<TKey, TValue>).FullName)
            {
                throw new NotSupportedException($"Type {objectType} unexpected, but got a {existingValue.GetType()}");
            }

            var dictionary = new Dictionary<TKey, TValue>();

            var jsonDictionary = JObject.Load(reader);

            foreach (var entry in jsonDictionary)
            {
                var jsonEntry = JsonConvert.SerializeObject(entry);
                var (key, value) = JsonConvert.DeserializeObject< KeyValuePair<TKey, TValue> >(jsonEntry);

                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}
