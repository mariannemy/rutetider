using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class RuterDeparturesFetcher
    {
        private readonly RuterSettings _settings;

        public RuterDeparturesFetcher()
        {
            _settings = new RuterSettings();
        }

        public Task<Departures> Fetch()
        {
            using(var httpClient = new HttpClient())
            {

            }
            return Task.FromResult(new Departures());
        }
    }
}
