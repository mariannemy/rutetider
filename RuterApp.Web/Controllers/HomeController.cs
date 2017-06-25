using RuterApp.Lib;
using RuterApp.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System;


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

     



        private static SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        public async Task<ActionResult> Index()
        {
            await Semaphore.WaitAsync();
            try
            {
                if(_stationApiResult == null)
                {
                    _stationApiResult = await _ruterStation.GetRuterData<RuterApiStationNameResult>(_settings.UrlGetStationName);
                }                
            }
            finally
            {
                Semaphore.Release(); 
            }
    
            _station.SetStationName(_stationApiResult);

            _departureApiResult = await _ruterStation.GetRuterData<RuterApiDataResult[]>(_settings.UrlGetDeparture);
           
            _departure1.SetDeparture(_settings, _departureApiResult, 1);
            _departure2.SetDeparture(_settings, _departureApiResult, 2);       

            var firstDeparture = new DeparturesInformation(_departure1);
            var secondDeparture = new DeparturesInformation(_departure2);
            var station = new StationInformation(_station);

     

            var _ruterReiseFacade = new RuterReiseFacade();
            List<Tuple<int, string>> _stationNames = await _ruterReiseFacade.GetAllStationNames();
                



            var ViewModel = new RuterViewModel
            {
                FirstDeparture = firstDeparture,
                SecondDeparture = secondDeparture,
                Station = station,
                StationName = _stationNames[0].Item2
               
            };             

            return View(ViewModel);
        }
    }
}