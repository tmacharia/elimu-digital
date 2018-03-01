using Common.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public string Room { get; set; }

        [JsonConverter(typeof(DayOfWeekConverter))]
        public DayOfWeek DayOfWeek { get; set; }

        [JsonConverter(typeof(TimeConverter))]
        public DateTime StartTime { get; set; }

        [JsonConverter(typeof(TimeConverter))]
        public DateTime EndTime { get; set; }
    }
}
