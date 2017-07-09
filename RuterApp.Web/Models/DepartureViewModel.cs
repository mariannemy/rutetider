using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RuterApp.Models
{
    public class DepartureViewModel
    {
        public List<Tuple<string, string>> LineAndDeparture { get; set; }
        public string StationName;
    }
}