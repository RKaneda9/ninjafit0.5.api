using Newtonsoft.Json;
using NinjaFit.Api.Services.Facebook;
using NinjaFit.Api.Support;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NinjaFit.Api.Services
{
    public class FacebookService : Service
    {
        private static HttpClient _client { get; set; }

        private static HttpClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri(Constants.Facebook.FacebookApiUrl);
                }

                return _client;
            }
        } 

        public static async Task<Models.FacebookFeed> TryGetFeedAsync()
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

        public static async Task<Models.FacebookFeed> GetFeedAsync()
        {
            string url = $"posts?access_token={Constants.Facebook.AccessToken}&limit={Constants.Facebook.PostLimit}&fields={Constants.Facebook.FieldsExt}";

            var result = await client.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Could not retrieve Facebook Feed. Response => StatusCode={(int)result.StatusCode}/{result.StatusCode}, ReasonPhrase={result.ReasonPhrase}");
            }

            var contentString = await result.Content.ReadAsStringAsync();
            var feed          = JsonConvert.DeserializeObject<FacebookFeed>(contentString);

            return feed.Convert();
        }
    }
}