using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class RuterReiseFacade
    {
        private RuterReiseApi _ruterReiseApi = new RuterReiseApi();

        public async Task<List<Tuple<int, string, string>>> GetAllStationsAndLines()
        {
            var _totalNumberOfLines = 5;
            StopsById[] _stopsById;
            var _stationsAndLinesOverview = new List<Tuple<int, string, string>>();
            var _spesificStationNames = new List<Tuple<int, string, string>>();

            for (int i = 0; i < _totalNumberOfLines; i++)
            {
                _stopsById = await _ruterReiseApi.Line_GetStopsByLineId(i);

                foreach (var station in _stopsById)
                {
                    _spesificStationNames.Add(new Tuple<int, string, string>(i, station.StationName, station.Id.ToString()));
                }

                _stationsAndLinesOverview.AddRange(_spesificStationNames);
            }
            return _stationsAndLinesOverview;
        }

        public IEnumerable<string> GetStationList(List<Tuple<int, string, string>> stationsAndLines)
        {
            IEnumerable<string> stationList = new List<string>();

            stationList = stationsAndLines.Select(x => x.Item2).ToList().Distinct().OrderBy(x => x);
            stationList = stationList.Select(x => x.Replace(x, Regex.Replace(x ,"(\\[.*\\])", "")));


            return stationList;
        }
        
    }
}