using Newtonsoft.Json;

namespace RuterApp.Lib
{
    public class StopsById
    {
        public int Id { get; set; }
        [JsonProperty("Name")]
        public string StationName { get; set; }
    }
}
