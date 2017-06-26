using RuterApp.Lib.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class RuterReiseApi
    {
        private AppSettings _setting = new AppSettings();
        private RuterDataProvider _ruterDataProvider = new RuterDataProvider();

        public async Task<StopsById[]> Line_GetStopsByLineId(int lineNumber)
        {            
            StopsById[] stationAndLine;
            stationAndLine = await _ruterDataProvider.GetRuterData<StopsById[]>(_setting.UrlGetStation(lineNumber));

            return stationAndLine;
        }

        public async Task<RuterApiDataResult[]> StopVisit_GetDepartures(int metroId)
        {
            RuterApiDataResult[] departureApiResult = new RuterApiDataResult[Constants.NUMBER_OF_DEPARTURES];
            departureApiResult = await _ruterDataProvider.GetRuterData<RuterApiDataResult[]>(_setting.UrlGetDeparture(metroId));

            return departureApiResult;           
        }

        public async Task<LinesForSpecificStops[]> Line_GetLinesByStopId(int stopId)
        {
            LinesForSpecificStops[] linesForSpecificStops;
            linesForSpecificStops = await _ruterDataProvider.GetRuterData<LinesForSpecificStops[]>(_setting.UrlGetLinesByStopId(stopId));

            return linesForSpecificStops;
        }
    }
}
