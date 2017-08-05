using Newtonsoft.Json;

namespace RuterApp.Lib.Apis
{
    public class LinesForSpecificStops
    {
        [JsonProperty("Id")]
        public int LineNumber { get;set;}
        public string Transportation { get; set; }
    }
}
