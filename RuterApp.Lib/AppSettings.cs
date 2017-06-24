using System.Configuration;

namespace RuterApp.Lib
{
    public class AppSettings
    {
        public string MetroId
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("StopId");
            }
        }

        public string Direction
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("Direction");
            }
        }

        public string UrlGetDeparture
        {
            get
            {
                return "http://reisapi.ruter.no/StopVisit/GetDepartures/" + MetroId + "?transporttypes=Metro";
            }
        }

        public string UrlGetStationName
        {
            get
            {
                return "http://reisapi.ruter.no/Place/GetStop/" + MetroId;
            }
        }

        public string UrlGetStation(int lineNumber)
        {         
                return "http://reisapi.ruter.no/Place/GetStopsByLineID/" + lineNumber;           
        }
    }
}
