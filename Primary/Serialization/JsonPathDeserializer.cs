using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Primary.Serialization
{
    internal class JsonPathDeserializer : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            object targetObj = existingValue ?? Activator.CreateInstance(objectType);

            foreach (var prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                          BindingFlags.Instance | BindingFlags.FlattenHierarchy
                                 ).Where(p => p.CanRead))
            {
                var pathAttribute = prop.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault();

                string jsonPath = pathAttribute?.PropertyName ?? prop.Name;
                var token = jsonObject.SelectToken(jsonPath);

                if (token != null && token.Type != JTokenType.Null)
                {
                    object value = null;

                    var converterAttribute = prop.GetCustomAttributes(true).OfType<JsonConverterAttribute>().FirstOrDefault();
                    if (converterAttribute != null)
                    {
                        var args = converterAttribute.ConverterParameters ?? Array.Empty<object>();
                        var converter = Activator.CreateInstance(converterAttribute.ConverterType, args) as JsonConverter;

                        if (converter != null && converter.CanRead)
                        {
                            using (var sr = new StringReader(token.ToString()))
                            using (var jr = new JsonTextReader(sr))
                            {
                                var tokenReader = token.CreateReader();
                                if (tokenReader.Read())
                                {
                                    value = converter.ReadJson(tokenReader, prop.PropertyType, value, serializer);
                                }
                            }
                        }
                    }
                    else
                    {
                        value = token.ToObject(prop.PropertyType, serializer);
                    }
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetCustomAttributes(true).OfType<JsonPathDeserializer>().Any();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


}
