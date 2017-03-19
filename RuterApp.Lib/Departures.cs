using System;

namespace RuterApp.Lib
{
    public class Departures
    {
        public string LineNumber { get; private set; }
        public string LineName { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public bool DepartureExists { get; private set; }

        private int _departureIndex;

        public void SetDeparture(AppSettings settings, RuterApiDataResult[] ruterApiDataResult, int departureNumber)
        {        
            _departureIndex = 0;
            foreach (RuterApiDataResult departureNumberInfo in ruterApiDataResult)
            {
                if (departureNumberInfo.GeneralInfo.DirectionRef == settings.Direction)
                {
                    LineNumber = departureNumberInfo.GeneralInfo.LineNumberRef;
                    LineName = departureNumberInfo.GeneralInfo.DestinationName;
                    DepartureTime = departureNumberInfo.GeneralInfo.RealTimeInfo.ExpectedDepartureTime;
                    _departureIndex++;
                }
                if (departureNumber == _departureIndex)
                {
                    DepartureExists = true;
                    break;
                }
            }
        }
    }
}
