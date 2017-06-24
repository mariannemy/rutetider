using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class StationNameFetcher
    {
        private AppSettings _setting = new AppSettings();
        private RuterDataProvider _ruterDataProvider = new RuterDataProvider();
   
        public async Task<List<Tuple<int, string>>> FetchData()
        {
            var _totalNumberOfLines = 5;
            var _stationsAndLinesOverview = new List<Tuple<int, string>>();
            var _spesificStationAndLineInfo = new List<Tuple<int, string>>();
            StopsById[] _stationAndLine;

            for (int i = 0; i < _totalNumberOfLines; i++)
            {
                _stationAndLine = await _ruterDataProvider.GetRuterData<StopsById[]>(_setting.UrlGetStation(i));

                foreach (var station in _stationAndLine)
                {
                    _spesificStationAndLineInfo.Add(new Tuple<int, string>(i, station.StationName));
                }

                _stationsAndLinesOverview.AddRange(_spesificStationAndLineInfo);
            }
          
            return _stationsAndLinesOverview;
        }
    }
}
