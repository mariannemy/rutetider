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

        public void SetCookie(CookieValues cookieValues)
        {
            //må endre til en konstant!!!
            var cookie = new HttpCookie("ruterAppUserId");
            cookie.Value = JsonConvert.SerializeObject(cookieValues);
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
        }

        public void OverwriteCookieValue()
        {

        }

        public CookieValues GetCookie()
        {
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);

            if (clientCookies.Exists(x => x.Contains("ruterAppUserId")))
            {
                var cookie = Request.Cookies.Get("ruterAppUserId");
                var cookieValues = JsonConvert.DeserializeObject<CookieValues>(Server.UrlEncode(cookie.Value));
                return cookieValues;
            }

            return null;
        }

        public void OverWriteCookieValue(CookieValues cookieValue)
        {
            //Kanskje putte urlenkoding inn her
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);
         

            var cookie = Request.Cookies.Get("ruterAppUserId");
            cookie.Value = JsonConvert.SerializeObject(cookieValue);
      

        }


        public async Task<ActionResult> Stations()

        {
            var ruterReiseFacade = new RuterReiseFacade();

            string _fromStation = Request["FromStation"];
            string metros = "seglkh, segs, sgeg,";

            //Test ny cookie
            var info = new CookieValues
            {
                StationId = Server.UrlEncode(_fromStation),
                MetroId = metros
            };



            HttpCookie cookieTest = new HttpCookie("cookieTest", JsonConvert.SerializeObject(info));
            Response.Cookies.Add(cookieTest);






            //sjekk og gi cookie, må vær emulig å skrive over cookie hvis man har en fra før av
            string[] clientCookies = Request.Cookies.AllKeys;
            List<string> clientCookiesList = new List<string>();
            clientCookiesList.AddRange(clientCookies);

            if (!clientCookiesList.Contains("ruterAppUserId"))
            {
                HttpCookie cookie = new HttpCookie("ruterAppUserId", Server.UrlEncode(_fromStation));
                cookie.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);

            }

            if (String.IsNullOrEmpty(_fromStation))
            {
                if (TempData["CustomError"] != null)
                {
                    ModelState.AddModelError("", TempData["CustomError"].ToString());
                }
                var cookie = Request.Cookies.Get("ruterAppUserId");
                _fromStation = Server.UrlDecode(cookie.Value);
            }




            List<Tuple<int, string, int>> _stationNames = await ruterReiseFacade.GetAllStationsAndLines();
            int fromStationId = _stationNames.Find(x => x.Item2.ToLower().Equals(_fromStation.ToLower())).Item3;

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
            //MÅ ha med validering
            //må ha logikk så ikke kan velge stasjoner + velg alle

            string selectedLines = Request["selectedLines"];

            var cookie = Request.Cookies.Get("cookieTest");
            var coInfo = new CookieValues();
            coInfo = JsonConvert.DeserializeObject<CookieValues>(Server.UrlDecode(cookie.Value));








            if (String.IsNullOrEmpty(selectedLines))
            {
                TempData["CustomError"] = "Du må velge en";

                return RedirectToAction("Stations");

            }

            string[] selectedLinesArray = selectedLines.Split(',');



            // int g = Int32.Parse(selectedLinesArray[0]);



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



            RuterReiseApi _ruterReiseApi = new RuterReiseApi();
            _departureApiResult = await _ruterReiseApi.StopVisit_GetDepartures(3010360);

            _departure1.SetDeparture(_settings, _departureApiResult, 1);
            _departure2.SetDeparture(_settings, _departureApiResult, 2);

            var firstDeparture = new DeparturesInformation(_departure1);
            var secondDeparture = new DeparturesInformation(_departure2);
            var station = new StationInformation(_station);




            var _ruterReiseFacade = new RuterReiseFacade();
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