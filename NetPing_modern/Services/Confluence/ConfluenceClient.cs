using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NetPing.Global.Config;
using Newtonsoft.Json.Linq;

namespace NetPing_modern.Services.Confluence
{
    internal class ConfluenceClient : IConfluenceClient
    {
        private IConfig _config;

        public ConfluenceClient(IConfig config)
        {
            _config = config;
        }

        public async Task<string> GetContenAsync(int id)
        {
            NetworkCredential credential = new NetworkCredential(_config.ConfluenceSettings.Login, _config.ConfluenceSettings.Password);
            var handler = new HttpClientHandler {Credentials = credential};
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(_config.ConfluenceSettings.Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response =
                    client.GetAsync(string.Format("wiki/rest/prototype/1/content/{0}?os_authType=basic", id));
                if (response.Result.IsSuccessStatusCode)
                {
                    StreamContent content = (StreamContent) response.Result.Content;
                    var task =  content.ReadAsStringAsync();
                    return task.Result;
                }
            }
            return string.Empty;
        }

        public async Task<int> GetContentBySpaceAndTitle(string spaceKey, string title)
        {
            NetworkCredential credential = new NetworkCredential(_config.ConfluenceSettings.Login, _config.ConfluenceSettings.Password);
            var handler = new HttpClientHandler { Credentials = credential };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(_config.ConfluenceSettings.Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response =
                    client.GetAsync(string.Format("wiki/rest/api/content?title={0}&spaceKey={1}&os_authType=basic", title, spaceKey));
                if (response.Result.IsSuccessStatusCode)
                {
                    StreamContent content = (StreamContent)response.Result.Content;
                    var task = content.ReadAsStringAsync();
                    string stringContent = task.Result;
                    dynamic results = JObject.Parse(stringContent);
                    if (results.results != null)
                    {
                        if (results.results.Type == JTokenType.Array && results.results.Count > 0)
                        {
                            if (results.results[0].id != null)
                            {
                                return int.Parse(results.results[0].id.Value);
                            }
                        }
                    }
                    return -1;
                }
            }
            return -1;
        }
    }
}