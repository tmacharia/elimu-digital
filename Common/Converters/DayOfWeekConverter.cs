using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Converters
{
    public class DayOfWeekConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DayOfWeek dayOfWeek = (DayOfWeek)value;

            writer.WriteValue(dayOfWeek.ToString());
        }
    }
}
