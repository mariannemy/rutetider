using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
