using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Yi.Database.Converters
{
    public class DeepDictionaryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(IDictionary).IsAssignableFrom(objectType) ||
                                                            TypeImplementsGenericInterface(objectType, typeof(IDictionary<,>));

        private static bool TypeImplementsGenericInterface(Type concreteType, Type interfaceType)
        {
            return concreteType.GetInterfaces()
                   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            var keys = (IEnumerable)type.GetProperty("Keys").GetValue(value, null);
            var values = (IEnumerable)type.GetProperty("Values").GetValue(value, null);
            var valueEnumerator = values.GetEnumerator();

            writer.WriteStartArray();
            foreach (var key in keys)
            {
                valueEnumerator.MoveNext();

                writer.WriteStartArray();
                serializer.Serialize(writer, key);
                serializer.Serialize(writer, valueEnumerator.Current);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new AccessViolationException();
        }
    }
    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => writer.WriteValue((bool)value ? 1 : 0);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => reader.Value.ToString() == "1";

        public override bool CanConvert(Type objectType) => objectType == typeof(bool);
    }
}