using RuterApp.Lib;
using RuterApp.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using RuterApp.Lib.Apis;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Newtonsoft.Json;

namespace RuterApp.Controllers
{
    public class HomeController : Controller
    {


        private readonly AppSettings _settings;
        private RuterDataProvider _ruterStation;
        private static RuterApiStationNameResult _stationApiResult;
        private Station _station;
        private Departures _departure1;
        private Departures _departure2;
        private RuterApiDataResult[] _departureApiResult = new RuterApiDataResult[Constants.NUMBER_OF_DEPARTURES];


        public HomeController()
        {
            _settings = new AppSettings();
            _ruterStation = new RuterDataProvider();
            _station = new Station();
            _departure1 = new Departures();
            _departure2 = new Departures();
        }


        public async Task<ActionResult> Index()
        {
            var ruterReiseFacade = new RuterReiseFacade();
            List<Tuple<int, string, int>> _stationNames = await ruterReiseFacade.GetAllStationsAndLines();

            var viewModel = new PickStationViewModel
            {
                Stations = ruterReiseFacade.GetStationList(_stationNames)
            };

            return View(viewModel);
        }



        public class CookieValues
        {
            public string StationId { get; set; }
            public string MetroId { get; set; }
        }

        public bool CookieExists()
        {
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);

            if (clientCookies.Exists(x => x.Contains("ruterAppUserId")))
            {
                return true;
            }
            return false;
        }

        public void SetCookie(CookieValues cookieValues)
        {
            //må endre til en konstant!!!
            var cookie = new HttpCookie("ruterAppUserId");
            cookie.Value = JsonConvert.SerializeObject(EncodeValues(cookieValues));
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
        }

        public CookieValues GetCookieValues()
        {
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);

            if (clientCookies.Exists(x => x.Contains("ruterAppUserId")))
            {
                var cookie = Request.Cookies.Get("ruterAppUserId");              
                var cookieValues = new CookieValues();
           
                cookieValues = JsonConvert.DeserializeObject<CookieValues>(cookie.Value);
                cookieValues.StationId = Server.UrlDecode(cookieValues.StationId);
                return cookieValues;
            }

            return null;
        }

        public CookieValues EncodeValues(CookieValues cookieValues)
        {
            cookieValues.MetroId = cookieValues.MetroId;
            cookieValues.StationId = Server.UrlEncode(cookieValues.StationId);
            return cookieValues;
        }

        public void OverWriteCookieValue(CookieValues cookieValues)
        {

            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);
            var cookie = Request.Cookies.Get("ruterAppUserId");
            cookie.Value = JsonConvert.SerializeObject(EncodeValues(cookieValues));
            Response.Cookies.Set(cookie);

        }


        public async Task<ActionResult> Stations()

        {
            var ruterReiseFacade = new RuterReiseFacade();

            string selectedStation = Request["FromStation"];


            //Test ny cookie
            var cookieValue = new CookieValues
            {
                StationId = selectedStation,
                MetroId = ""
            };

            // CookieValues cookieValues = new CookieValues();

            if (!CookieExists())
            {
                if (String.IsNullOrEmpty(selectedStation))
                {
                    return RedirectToAction("Index");
                }
                SetCookie(cookieValue);
            }
            else
            {
                if (String.IsNullOrEmpty(selectedStation))
                {
                    cookieValue = GetCookieValues();
                    if (cookieValue == null)
                    {
                        return RedirectToAction("Index");
                    }
                    selectedStation = cookieValue.StationId;

                }
                else
                {
                    OverWriteCookieValue(cookieValue);
                }

            }

            if (String.IsNullOrEmpty(selectedStation))
            {
                if (TempData["CustomError"] != null)
                {
                    ModelState.AddModelError("", TempData["CustomError"].ToString());
                }
            }

            List<Tuple<int, string, int>> _stationNames = await ruterReiseFacade.GetAllStationsAndLines();

            int fromStationId = _stationNames.Find(x => x.Item2.ToLower().Equals(selectedStation.ToLower())).Item3;

            Dictionary<string, string> linesServingStation = await ruterReiseFacade.GetLinesServingStop(fromStationId);

            string chooseAllStations = String.Empty;
            foreach (var lines in linesServingStation)
            {
                chooseAllStations = chooseAllStations + lines.Key + ";";
            }
            linesServingStation.Add(chooseAllStations, "Velg alle");



            var viewModel = new PickLinesViewModel
            {
                LinesServingStation = linesServingStation
            };

            return View(viewModel);
        }


        private static SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);


        public async Task<ActionResult> Show(PickLinesViewModel lines)
        {

            string selectedLines = Request["selectedLines"];
            string selectedStation;

            var cookie = new CookieValues();

            //No chosen lines
            if (String.IsNullOrEmpty(selectedLines))
            {
                TempData["CustomError"] = "Du må velge en";

                return RedirectToAction("Stations");

            }

            //Redirect if no cookie exist og cookie values not set
            if (!CookieExists())
            {
                return RedirectToAction("Index");
            }
            else
            {
                cookie = GetCookieValues();
                if (cookie == null)
                {
                    return RedirectToAction("Index");
                }
                selectedStation = cookie.StationId;
            }

            //Overwrite with selected lines
            var cookieValues = new CookieValues
            {
                StationId = cookie.StationId,
                MetroId = selectedLines
            };

            OverWriteCookieValue(cookieValues);

            var _ruterReiseFacade = new RuterReiseFacade();

            List<Tuple<int, string, int>> allStationsAndLines = await _ruterReiseFacade.GetAllStationsAndLines();
            int selectedStationId = allStationsAndLines.Find(x => x.Item2.ToLower().Equals(selectedStation.ToLower())).Item3;

            //*******Ta inn stationsID og liste over metroID - vise de to neste*******
            RuterApiDataResult[] departureApiResults;
            RuterReiseApi _ruterReiseApi = new RuterReiseApi();

            departureApiResults = await _ruterReiseApi.StopVisit_GetDepartures(selectedStationId);





            string[] selectedLinesArray = selectedLines.Split(',');

            await Semaphore.WaitAsync();
            try
            {
                if (_stationApiResult == null)
                {
                    _stationApiResult = await _ruterStation.GetRuterData<RuterApiStationNameResult>(_settings.UrlGetStationName);
                }
            }
            finally
            {
                Semaphore.Release();
            }

            _station.SetStationName(_stationApiResult);


            
            _departureApiResult = await _ruterReiseApi.StopVisit_GetDepartures(selectedStationId);

            _departure1.SetDeparture(_settings, _departureApiResult, 1);
            _departure2.SetDeparture(_settings, _departureApiResult, 2);

            var firstDeparture = new DeparturesInformation(_departure1);
            var secondDeparture = new DeparturesInformation(_departure2);
            var station = new StationInformation(_station);




            List<Tuple<int, string, int>> _stationNames = await _ruterReiseFacade.GetAllStationsAndLines();



            List<string> stationList = new List<string>();

            foreach (var stat in _stationNames)
            {
                stationList.Add(stat.Item2);
            }






            var ViewModel = new RuterViewModel
            {
                FirstDeparture = firstDeparture,
                SecondDeparture = secondDeparture,
                Station = station,



            };

            return View(ViewModel);
        }
    }
}