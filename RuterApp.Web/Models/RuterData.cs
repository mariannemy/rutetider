using RuterApp.Lib;
using System;

namespace RuterApp.Models
{

    public class DeparturesInformation
    {
        public DeparturesInformation(Departures departure)
        {
            LineNumber = departure.LineNumber;
            LineName = departure.LineName;
            DepartureTime = departure.DepartureTime;
            DepartureExists = departure.DepartureExists;
        }

        public string LineNumber { get; set; }
        public string LineName { get; set; }
        public DateTime DepartureTime { get; set; }
        public bool DepartureExists;   


        public string getMinutes()
        {
            DateTime timeNow = DateTime.Now;
            var minutesPassed = Math.Floor((DepartureTime - timeNow).TotalMinutes+0.30);
   
            if (minutesPassed == 0)
            {
                return "NÅ";
            }
            if (minutesPassed >= 60)
            {
                return "ingen";
            }

            return minutesPassed.ToString();
        }
    }

    public class StationInformation
    {

        public StationInformation(Station station)
        {
            Name = station.Name;
         }
        public string Name { get; set; }
    }
}
   
