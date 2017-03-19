using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    public class RuterDataProvider
    {
        private HttpClient _client;
        private HttpClientProvider _clientProvider;

        public RuterDataProvider()
        {
            _clientProvider = new HttpClientProvider();
        }

        public async Task<T> GetRuterData<T>(string url)
        {
            _client = _clientProvider.GetHttpClient();
            Task<string> getStringTask = _client.GetStringAsync(url);
            string urlContent = await getStringTask;
            T result = JsonConvert.DeserializeObject<T>(urlContent);

            return result;
        }
    }
}
