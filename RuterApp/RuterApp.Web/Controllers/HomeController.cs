using RuterApp.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RuterApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly RuterDeparturesFetcher _fetcher;

        public HomeController()
        {
            _fetcher = new RuterDeparturesFetcher();
        }

        // GET: Home
        public async Task<ActionResult> Index()
        {
            var ruterResult = await _fetcher.Fetch(); // Bare et eksempel på hvordan man kan kalle en metode på en ekstern klasse, for å hente rutetider
            return View();
        }
    }
}