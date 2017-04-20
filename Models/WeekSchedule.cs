using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace NinjaFit.Api.Models
{
    public class WeekSchedule
    {
        public string Start { get; set; }
        public string End   { get; set; }

        public List<DaySchedule> Days { get; set; }

        public WeekSchedule()
        {
            Days = new List<DaySchedule>();
        }
    }

    public class DaySchedule
    {
        public int Date { get; set; }
        public bool IsDefault { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DayOfWeek Day { get; set; }

        public List<EventBlock> Blocks { get; set; }

        public DaySchedule()
        {
            Blocks = new List<EventBlock>();
        }
    }

    public class EventBlock
    {
        public string Start { get; set; }
        public string End   { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EventType Type { get; set; }

        public string Title { get; set; }
    }

    public enum EventType
    {
        Open,
        Event
    }
}