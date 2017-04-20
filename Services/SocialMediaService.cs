using System.Threading.Tasks;

namespace NinjaFit.Api.Services
{
    public class SocialMediaService : Service
    {
        public static async Task<Models.SocialMediaFeeds> TryGetFeedsAsync()
        {
            return new Models.SocialMediaFeeds
            {
                Facebook  = await  FacebookService.TryGetFeedAsync(),
                Instagram = await InstagramService.TryGetFeedAsync()
            };
        }
    }
}