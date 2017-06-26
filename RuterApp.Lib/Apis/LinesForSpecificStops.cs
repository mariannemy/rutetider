using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
