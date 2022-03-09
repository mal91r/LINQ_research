using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Class
{
    internal record WeatherEvents
    {
        [Flags]
        public enum Types
        {
            Snow,
            Cold,
            Hail,
            Fog,
            Precipitation,
            Rain,
            Storm
        }
        [Flags]
        public enum Severities
        {
            UNK,
            Light,
            Moderate,
            Severe, 
            Heavy,
            Other
        }
        public string EventId { get; init; }
        public Types Type { get; init; }
        public Severities Severity { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public string TimeZone { get; init; }
        public string AirportCode { get; init; }
        public double LocationLat { get; init; }
        public double LocationLng { get; init; }
        public string City { get; init; }
        public string County { get; init; }
        public string State { get; init; }
        public string ZipCode { get; init; }

        public WeatherEvents(List<string> weatherEvent)
        {
            EventId = weatherEvent[0];
            Type = (Types)Enum.Parse(typeof(Types), weatherEvent[1]);
            Severity = (Severities)Enum.Parse(typeof(Severities), weatherEvent[2]);
            StartTime = DateTime.Parse(weatherEvent[3]);
            EndTime = DateTime.Parse(weatherEvent[4]);
            TimeZone = weatherEvent[5];
            AirportCode = weatherEvent[6];
            LocationLat = double.Parse(weatherEvent[7]);
            LocationLng = double.Parse(weatherEvent[8]);
            City = weatherEvent[9];
            County = weatherEvent[10];
            State = weatherEvent[11];
            ZipCode = weatherEvent[12];
        }

        public static string[] Parse(string Event)
        {
            return Event.Split('\u002C');
        }
    }
}
