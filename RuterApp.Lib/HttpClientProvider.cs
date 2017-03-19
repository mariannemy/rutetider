using System.Net.Http;

namespace RuterApp.Lib
{
    class HttpClientProvider
    {
        public HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            return client;
        }
    }
}
