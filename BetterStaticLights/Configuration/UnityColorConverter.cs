using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace BetterStaticLights.Configuration
{
    internal class UnityColorConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);
            if (token is not JArray array)
            {
                throw new JsonReaderException($"Could not read {objectType} from json; expected a json array but got {token.Type}");
            }

            return new Color(
                array[0].ToObject<float>(),
                array[1].ToObject<float>(),
                array[2].ToObject<float>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType().Equals(typeof(Color)))
            {
                Color colorValue = (Color)value;

                writer.WriteStartArray();
                writer.WriteValue(colorValue.r);
                writer.WriteValue(colorValue.g);
                writer.WriteValue(colorValue.b);
                writer.WriteEndArray();
            }
            else
            {
                throw new JsonReaderException("Invalid type for JsonConverter: expected 'Color', got " + value.GetType());
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
