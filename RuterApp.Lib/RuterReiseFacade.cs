using RuterApp.Lib.Apis;
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

        public async Task<IEnumerable<string>> GetLinesServingStop(int stopId)
        {
            LinesForSpecificStops[] linesForSpecificStops;
            linesForSpecificStops = await _ruterReiseApi.Line_GetLinesByStopId(stopId);

            List<int> linesForStop = linesForSpecificStops.Where(x => x.Transportation.Equals("8")).Select(x => x.LineNumber).ToList();

            var AllLines = new List<Tuple<int, string>>
            {
                Tuple.Create(1, "Frognerseteren"),
                Tuple.Create(1, "Bergkrystallen"),
                Tuple.Create(2, "Østerås"),
                Tuple.Create(2, "Ellingsrudåsen"),
                Tuple.Create(3, "Kolsås"),
                Tuple.Create(3, "Mortensrud"),
                Tuple.Create(4, "Vestli"),
                Tuple.Create(4, "Bergkrystallen"),
                Tuple.Create(5, "Sognsvann"),
                Tuple.Create(5, "Vestli")
            };

            List<string> linesServingStop = new List<string>();

            foreach (var line in linesForStop)
            {
                linesServingStop.AddRange(AllLines.Where(x => x.Item1 == line).Select(x => x.Item2));
            }            

            return linesServingStop;
        }


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