using Newtonsoft.Json;
using System;

namespace RuterApp.Lib
{
    public class RuterApiDataResult
    {
        [JsonProperty("MonitoredVehicleJourney")]
        public LineInfo GeneralInfo { get; set; }
     
    }

    public class LineInfo
    {
        [JsonProperty("LineRef")]
        public string LineNumberRef { get; set; }
        public string DirectionRef { get; set; }
        public string DestinationName { get; set; }
        public string VehicleMode { get; set; }
        [JsonProperty("MonitoredCall")]
        public MonitoredInfo RealTimeInfo { get; set; }
    }

    public class MonitoredInfo
    {
        public DateTime ExpectedDepartureTime;
    }
}
