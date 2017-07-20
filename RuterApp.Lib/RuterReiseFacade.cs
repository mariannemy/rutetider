using RuterApp.Lib.Apis;
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

        public async Task<Dictionary<string, string>> GetLinesServingStop(int stopId)
        {
            RuterApiDataResult[] apiResult = await _ruterReiseApi.StopVisit_GetDepartures(stopId);

            var linesServingStop = new Dictionary<string, string>();

            string stationAndLineTextString;

            foreach (var station in apiResult)
            {
                stationAndLineTextString = "Linje " + station.GeneralInfo.LineNumberRef + " retning " + station.GeneralInfo.DestinationName;

                if (!linesServingStop.ContainsKey(station.GeneralInfo.DestinationRef.ToString()))
                {
                    linesServingStop.Add(station.GeneralInfo.DestinationRef.ToString(), stationAndLineTextString);
                }
            }
            return linesServingStop;
        }


        public async Task<List<Tuple<int, string, int>>> GetAllStationsAndLines()
        {
            var _totalNumberOfLines = 5;
            StopsById[] _stopsById;
            var _stationsAndLinesOverview = new List<Tuple<int, string, int>>();
            var _spesificStationNames = new List<Tuple<int, string, int>>();

            for (int i = 0; i < _totalNumberOfLines; i++)
            {
                _stopsById = await _ruterReiseApi.Line_GetStopsByLineId(i);

                foreach (var station in _stopsById)
                {
                    _spesificStationNames.Add(new Tuple<int, string, int>(i, StringUtils.GetNormalizedStationName(station.StationName), station.Id));
                }

                _stationsAndLinesOverview.AddRange(_spesificStationNames);
            }
            return _stationsAndLinesOverview;
        }

        public IEnumerable<string> GetStationList(List<Tuple<int, string, int>> stationsAndLines)
        {
            IEnumerable<string> stationList = new List<string>();

            stationList = stationsAndLines.Select(x => x.Item2).ToList().Distinct().OrderBy(x => x);
            stationList = stationList.Select(stationName => StringUtils.GetNormalizedStationName(stationName));

            return stationList;
        }

        public async Task<List<Tuple<string, string>>> GetDeparturesInMinutes(int selectedStationId, string[] selectedMetrosId)
        {
            List<Tuple<string, string>> metroNameAndDeparture = new List<Tuple<string, string>>();
            RuterApiDataResult[] departureApiResults = await _ruterReiseApi.StopVisit_GetDepartures(selectedStationId);

            double minutesPassed;
            string minutesToDeparture;

            foreach (var departures in departureApiResults)
            {
                for (int i = 0; i < selectedMetrosId.Length; i++)

                {
                    if (departures.GeneralInfo.DestinationRef.Equals(Int32.Parse(selectedMetrosId[i])))
                    {

                        minutesPassed = Math.Floor((departures.GeneralInfo.RealTimeInfo.ExpectedDepartureTime - DateTime.Now).TotalMinutes + 0.30);

                        minutesToDeparture = minutesPassed.ToString();

                        if (minutesPassed == 0)
                        {
                            minutesToDeparture = "NÅ";
                        }
                        if (minutesPassed >= 60)
                        {
                            minutesToDeparture = "Ingen";
                        }
                        metroNameAndDeparture.Add(new Tuple<string, string>(StringUtils.GetNormalizedStationName
                                                    (departures.GeneralInfo.DestinationName), minutesToDeparture));
                    }
                }
            }
            return metroNameAndDeparture;
        }

        public async Task<List<Tuple<string, string>>> GetDepartures(string selectedStation, string selectedLines)
        {

            List<Tuple<int, string, int>> allStationsAndLines = await GetAllStationsAndLines();
            int selectedStationId = allStationsAndLines.Find(x => x.Item2.ToLower().Equals(selectedStation.ToLower())).Item3;

            RuterApiDataResult[] departureApiResults;

            departureApiResults = await _ruterReiseApi.StopVisit_GetDepartures(selectedStationId);

            return await GetDeparturesInMinutes(selectedStationId, selectedLines.Split(','));
        }





    }
}