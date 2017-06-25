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
            StopsById[] _stationAndLine;
            _stationAndLine = await _ruterDataProvider.GetRuterData<StopsById[]>(_setting.UrlGetStation(lineNumber));

            return _stationAndLine;
        }

        public async Task<RuterApiDataResult[]> StopVisit_GetDepartures(int metroId)
        {
            RuterApiDataResult[] _departureApiResult = new RuterApiDataResult[Constants.NUMBER_OF_DEPARTURES];

            _departureApiResult = await _ruterDataProvider.GetRuterData<RuterApiDataResult[]>(_setting.UrlGetDeparture(metroId));

            return _departureApiResult;           
        }
    }
}
