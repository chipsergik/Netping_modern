using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetPing.Global.Config;
using Newtonsoft.Json.Linq;

namespace NetPing_modern.Services.Confluence
{
    internal class ConfluenceClient : IConfluenceClient
    {
        private IConfig _config;
        private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();

        public ConfluenceClient(IConfig config)
        {
            _config = config;
        }

        private async Task<string> GetContentAsync(int id, Func<int, string, string> parser)
        {
            if (_cache.ContainsKey(id))
            {
                return parser(id, _cache[id]);
            }

            NetworkCredential credential = new NetworkCredential(_config.ConfluenceSettings.Login, _config.ConfluenceSettings.Password);
            var handler = new HttpClientHandler { Credentials = credential };
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(_config.ConfluenceSettings.Url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response =
                    client.GetAsync(string.Format("wiki/rest/api/content/{0}?expand=body.view&os_authType=basic", id));
                if (response.Result.IsSuccessStatusCode)
                {
                    StreamContent content = (StreamContent)response.Result.Content;
                    var task = content.ReadAsStringAsync();
                    string result = task.Result;
                    return parser(id, result);
                }
            }
            return string.Empty;
        }

        public async Task<string> GetContenAsync(int id)
        {
            return await GetContentAsync(id, ParseResult);
        }

        public async Task<string> GetContentTitleAsync(int id)
        {
            return await GetContentAsync(id, ParseTitle);
        }

        private string ParseTitle(int id, string content)
        {
            if (!_cache.ContainsKey(id))
            {
                _cache[id] = content;
            }

            if (IsJson(content))
            {
                dynamic obj = JObject.Parse(content);
                if (obj.type == "page")
                {
                    return obj.title;
                }
                throw new NotImplementedException(string.Format("The type {0} is not implemented", obj.type));
            }
            return content;
        }

        private string ParseResult(int id, string result)
        {
            if (!_cache.ContainsKey(id))
            {
                _cache[id] = result;
            }
            
            if (IsJson(result))
            {
                return ParseJson(result);
            }
            return result;
        }

        private string ParseJson(string result)
        {
            dynamic obj = JObject.Parse(result);
            if (obj.type == "page")
            {
                return ParsePage(obj);
            }
            throw new NotImplementedException(string.Format("The type {0} is not implemented", obj.type));
        }

        private string ParsePage(dynamic page)
        {
            if (page.link != null)
            {
                if (page.link.Type == JTokenType.Array && page.link.Count > 0)
                {
                    return page.body.value;
                }
            }
            else if (page.body != null && page.body.view != null)
            {
                return page.body.view.value;
            }
            return string.Empty;
        }

        private bool IsJson(string result)
        {
            return result.StartsWith("{") && result.EndsWith("}");
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

        private readonly Regex _contentIdRegex = new Regex(@"pageId=(?<id>\d+)");

        public int? GetContentIdFromUrl(string url)
        {
            var mc = _contentIdRegex.Matches(url);
            if (mc.Count > 0)
            {
                Match m = mc[0];
                if (m.Success)
                {
                    Group group = m.Groups["id"];
                    int id = int.Parse(group.Value);

                    return id;
                }
            }
            return null;
        }
    }
}