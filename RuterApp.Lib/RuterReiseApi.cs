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
    }
}
