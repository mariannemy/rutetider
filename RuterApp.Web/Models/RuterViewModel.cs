

using System.Collections.Generic;

namespace RuterApp.Models
{
    public class RuterViewModel
    {
        public DeparturesInformation FirstDeparture { get; set; }
        public DeparturesInformation SecondDeparture { get; set; }
        public StationInformation Station { get; set; }    
        public string StationName { get; set; }

        public string Test { get; set; }
        public string Test2 { get; set; }
     
    }
}