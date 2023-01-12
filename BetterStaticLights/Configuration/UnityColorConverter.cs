using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;

namespace BetterStaticLights.Configuration
{
    public class UnityColorConverter : JsonConverter
    {
        private readonly Type[] _types;

        public UnityColorConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
