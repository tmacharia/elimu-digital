using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Converters
{
    public class TimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new DateTime();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(DateTime.Parse(value.ToString()).ToString("h:mm tt"));
        }
    }
}
