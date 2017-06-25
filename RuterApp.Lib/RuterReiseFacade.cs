using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class RuterReiseFacade
    {
        private RuterReiseApi _ruterReiseApi = new RuterReiseApi();

        public async Task<List<Tuple<int, string>>> GetAllStationNames()
        {
            var _totalNumberOfLines = 5;
            StopsById[] _stopsById;
            var _stationsAndLinesOverview = new List<Tuple<int, string>>();
            var _spesificStationNames = new List<Tuple<int, string>>();

            for (int i = 0; i < _totalNumberOfLines; i++)
            {
                _stopsById = await _ruterReiseApi.Line_GetStopsByLineId(i);

                foreach (var station in _stopsById)
                {
                    _spesificStationNames.Add(new Tuple<int, string>(i, station.StationName));
                }

                _stationsAndLinesOverview.AddRange(_spesificStationNames);
            }
            return _stationsAndLinesOverview;
        }
    }
}