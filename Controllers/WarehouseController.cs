using NinjaFit.Api.Extensions;
using NinjaFit.Api.Models;
using NinjaFit.Api.Services;
using NinjaFit.Api.Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace NinjaFit.Api.Controllers
{
    [RoutePrefix("warehouse")]
    public class WarehouseController : ApiController
    {
        public WarehouseController() { }

        [Route("schedule")]
        [HttpGet]
        public async Task<object> GetSchedule(int? start = null)
        {
            DateTime startDate = start.HasValue && Utils.IsValidDateKey(start.Value)
                ? Utils.ToDate(start.Value).StartOfWeek()
                : DateTime.UtcNow.StartOfWeek();

            DateTime endDate = startDate.EndOfWeek();

            return await ScheduleService.TryGetWeekScheduleAsync(startDate, endDate);
        }
        
        [Route("social")]
        [HttpGet]
        public async Task<object> GetSocialMediaFeeds()
        {
            return await SocialMediaService.TryGetFeedsAsync();
        }
        
        [Route("wods")]
        [HttpGet]
        public async Task<object> GetWods()
        {
            return await WodService.TryGetWodsAsync();
        }

        [Route("message")]
        [HttpPost]
        public object SendMessage(Message message)
        {
            return MailService.Send(message);
        }

        [Route("login/attempt")]
        [HttpPost]
        public async Task<object> TryLogin(LoginCredientials creds)
        {
            Log.Debug($"New Login Attempt Request from UserName={creds.UserName}, Password={creds.Password}");

            return await RxService.TryLoginAsync(creds);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginCredientials creds)
        {
            if (creds == null) { throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest); }

            Log.Debug($"New Login Request from UserName={creds.UserName}, Password={(Constants.Login.LogCreds ? creds.Password : "----")}");

            var result = await RxService.TryLoginAsync(creds);

            string redirectUri;

            switch (result.Result)
            {
                case LoginResult.ValidCredientials:   redirectUri = $"{Constants.Rx .BaseUrl}{Constants.Rx .LoginExt}";                         break;
                case LoginResult.InvalidCredientials: redirectUri = $"{Constants.App.BaseUrl}{Constants.App.LoginExt}?creds=invalid";           break;
                case LoginResult.LockedOut:           redirectUri = $"{Constants.App.BaseUrl}{Constants.App.LoginExt}?creds=invalid&lockedout"; break;
                default:                              redirectUri = $"{Constants.Rx .BaseUrl}{Constants.Rx .LoginExt}";                         break;
            }

            Log.Debug($"Redirecting login request to url={redirectUri}");

            return Redirect(redirectUri);
        }

        [Route("forgot")]
        [HttpPost]
        public async Task<object> ForgotPassword(ForgotPasswordRequest request)
        {
            if (request == null) { throw new HttpResponseException(System.Net.HttpStatusCode.BadRequest); }

            Log.Debug($"New Forgot Password Request from UserName={request.UserName}");

            return await RxService.TryForgotPasswordAsync(request);
        }
    }
}
