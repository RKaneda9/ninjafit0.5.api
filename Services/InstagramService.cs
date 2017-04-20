using Newtonsoft.Json;
using NinjaFit.Api.Services.Instagram;
using NinjaFit.Api.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NinjaFit.Api.Services
{
    public class InstagramService : Service
    {
        private static HttpClient _client { get; set; }

        private static HttpClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri(Constants.Instagram.PageUrl);
                }

                return _client;
            }
        }

        public static async Task<Models.InstagramFeed> TryGetFeedAsync()
        {
            try
            {
                return await GetFeedAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public static async Task<Models.InstagramFeed> GetFeedAsync()
        {
            var result = await client.GetAsync("");

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Could not retrieve Instagram Feed. Response => StatusCode={(int)result.StatusCode}/{result.StatusCode}, ReasonPhrase={result.ReasonPhrase}");
            }

            var contentString = await result.Content.ReadAsStringAsync();

            return ParseInstagramFeed(contentString).Convert();
        }

        public static InstagramFeed ParseInstagramFeed(string contentString)
        {
            InstagramFeed feed = new InstagramFeed();

            string contentLocationStart = "window._sharedData = ";
            string contentLocationEnd   = ";</script>";

            int dataStartIndex = contentString.IndexOf(contentLocationStart);

            if (dataStartIndex < 0) { throw new Exception($"Could not find data on page. String: '{contentLocationStart}' was missing."); }

            dataStartIndex += (contentLocationStart).Length;

            string dataString = contentString.Substring(dataStartIndex);

            int dataEndIndex = dataString.IndexOf(contentLocationEnd);

            dataString = dataString.Substring(0, dataEndIndex);

            return JsonConvert.DeserializeObject<InstagramFeed>(dataString);
        }
    }
}