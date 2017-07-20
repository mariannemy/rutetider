using RuterApp.Lib;
using RuterApp.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;
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
        private RuterReiseFacade _ruterReiseFacade;


        public HomeController()
        {
            _ruterReiseFacade = new RuterReiseFacade();
            _settings = new AppSettings();
            _ruterStation = new RuterDataProvider();
            _station = new Station();

        }


        public async Task<ActionResult> Index()
        {
           /* if (CookieExists())
            {
                var cookie = GetCookieValues();

                if(!String.IsNullOrEmpty(cookie.MetroId) && !String.IsNullOrEmpty(cookie.MetroId))
                {
                    return RedirectToAction("Show");
                }
            }*/

            List<Tuple<int, string, int>> _stationNames = await _ruterReiseFacade.GetAllStationsAndLines();

            var viewModel = new PickStationViewModel
            {
                Stations = _ruterReiseFacade.GetStationList(_stationNames)
            };

            return View(viewModel);
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
                chooseAllStations = chooseAllStations + lines.Key + ",";
            }
            if (linesServingStation.Count > 1)
            {
                linesServingStation.Add(chooseAllStations.TrimEnd(','), "Velg alle");
            }
        



            var viewModel = new PickLinesViewModel
            {
                LinesServingStation = linesServingStation
            };

            return View(viewModel);
        }





        public async Task<ActionResult> Show(PickLinesViewModel lines)
        {

            string selectedLines = Request["selectedLines"];
            string selectedStation;

            var cookie = new CookieValues();

            var cookieValues = new CookieValues();
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
                else if (cookie.MetroId == null)
                {
                    TempData["CustomError"] = "Du må velge en";

                    return RedirectToAction("Stations");
                }
                else if (cookie.StationId == null)
                {
                    return RedirectToAction("Index");
                }
                else if (String.IsNullOrEmpty(selectedLines))
                {
                    selectedLines = cookie.MetroId;
                }
                else if (!String.IsNullOrEmpty(selectedLines))
                {
                    var newCookieValues = new CookieValues();
                    newCookieValues.MetroId = selectedLines;
                    newCookieValues.StationId = cookie.StationId;
                    OverWriteCookieValue(newCookieValues);
                }
            }
            selectedStation = cookie.StationId;


            List<Tuple<string, string>> metroNameAndDeparture = await _ruterReiseFacade.GetDepartures(selectedStation, selectedLines);
        
            DepartureViewModel viewModel = new DepartureViewModel
            {
                LineAndDeparture = metroNameAndDeparture,
                StationName = selectedStation
            };

            return View("Show", viewModel);


        }

        public class CookieValues
        {
            public string StationId { get; set; }
            public string MetroId { get; set; }
        }

        public bool CookieExists()
        {
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);

            if (clientCookies.Exists(x => x.Contains(Constants.COOKIE_NAME)))
            {
                return true;
            }
            return false;
        }

        public void SetCookie(CookieValues cookieValues)
        {
            //må endre til en konstant!!!
            var cookie = new HttpCookie(Constants.COOKIE_NAME);
            cookie.Value = JsonConvert.SerializeObject(EncodeValues(cookieValues));
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
        }

        public CookieValues GetCookieValues()
        {
            List<string> clientCookies = new List<string>(Request.Cookies.AllKeys);

            if (clientCookies.Exists(x => x.Contains(Constants.COOKIE_NAME)))
            {
                var cookie = Request.Cookies.Get(Constants.COOKIE_NAME);
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
            var cookie = Request.Cookies.Get(Constants.COOKIE_NAME);
            cookie.Value = JsonConvert.SerializeObject(EncodeValues(cookieValues));
            Response.Cookies.Set(cookie);

        }
    }

}