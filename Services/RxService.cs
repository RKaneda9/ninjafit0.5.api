using NinjaFit.Api.Extensions;
using NinjaFit.Api.Models;
using NinjaFit.Api.Services.Rx;
using NinjaFit.Api.Support;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace NinjaFit.Api.Services
{
    public class RxService : Service
    {
        private const string  INCORRECT_LOGIN_TEXT = "<b>Name or password is not correct, or Ninja is inactive.</b>";
        private const string LOCKED_OUT_LOGIN_TEXT = "<b>Too many failed login attempts within a short time, Ninja is locked for one hour.</b>";

        private const string INVALID_CREDS_FORGOT_PASSWORD_TEXT = "No email or active profile found for ";
        private const string   VALID_CREDS_FORGOT_PASSWORD_TEXT = "Password sent to email on file for ";


        private static HttpClient _client { get; set; }

        private static HttpClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.BaseAddress = new Uri(Constants.Rx.BaseUrl);
                }

                return _client;
            }
        }

        public static async Task<WodSummary> GetWodAsync(int index = 1)
        {
            try
            {
                var response = await client.GetStringAsync($"{Constants.Wod.ExtUrl}?wodindex={index}&todayonly=");

                return ParseWodFromResponse(response);
            }
            catch (Exception ex)
            {
                Log.Info(ex, "Could not parse WOD from Rx website. The most likely case is that no WOD exists for today.");
            }

            return null;
        }

        // TODO: if request range is within the next 7 days
        public static async Task<List<DaySchedule>> TryGetClassScheduleAsync()
        {
            try
            {
                var response = await client.GetStringAsync(Constants.Schedule.RxExtUrl);

                return ParseClassScheduleFromResponse(response);
            }
            catch (Exception ex)
            {
                Log.Info(ex, "Could not retrieve and parse Class Schedule from rx gym software.");
            }

            return new List<DaySchedule>();
        }

        public static async Task<ForgotPasswordResponse> TryForgotPasswordAsync(ForgotPasswordRequest request)
        {
            string url = $"{Constants.Rx.LoginExt}?page=login&forgot={request.UserName}";

            ForgotPasswordResponse response = new ForgotPasswordResponse
            {
                RedirectLink = $"{Constants.Rx.BaseUrl}{url}",
                Result       = ForgotPasswordResult.Unknown
            };

            try
            {
                var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    string responseString = await result.Content.ReadAsStringAsync();
                    if (Constants.Rx.LogForgotPwdesult) { Log.Debug(responseString); }
                    response.Result = ParseForgotPasswordResult(responseString, request.UserName);
                    return response;
                }
                
                throw new Exception($"RxService::ForgotPasswordAsync Non success status code returned. Status={(int)result.StatusCode}/{result.StatusCode}, Reason={result.ReasonPhrase}, Url={client.BaseAddress}{url}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred when attempting to send Forgot Password request. Could not determine result of send Forgot Password.");
            }

            return response;
        } 

        public static async Task<LoginResponse> TryLoginAsync(LoginCredientials creds)
        {
            var response = new LoginResponse
            {
                Result       = LoginResult.Unknown,
                RedirectLink = $"{client.BaseAddress}{Constants.Rx.LoginExt}"
            };

            try
            {
                var form = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("membername", creds.UserName),
                    new KeyValuePair<string, string>("password",   creds.Password),
                    new KeyValuePair<string, string>("page",       "login")
                };
                
                var content = new FormUrlEncodedContent(form);

                var result = await client.PostAsync(Constants.Rx.LoginExt, content);

                if (result.IsSuccessStatusCode)
                {
                    string responseString = await result.Content.ReadAsStringAsync();
                    if (Constants.Rx.LogLoginResult) { Log.Debug(responseString); }
                    response.Result = ParseValidLoginCredsFromResponse(responseString, creds.UserName);
                    return response;
                }
                
                throw new Exception($"RxService::TryLoginAsync Non success status code returned. Status={(int)result.StatusCode}/{result.StatusCode}, Reason={result.ReasonPhrase}, Url={client.BaseAddress}{Constants.Rx.LoginExt}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred when posting user information to Rx's Login url. Could not determine if valid credientials");
            }

            return response;
        }

        private static ForgotPasswordResult ParseForgotPasswordResult(string response, string userName)
        {
            int resultContainerIndex = response.FindContentIndex("id='container_body_full'");
            int footerContainerIndex = response.FindContentIndex("id='container_footer'");

            string resultContent = footerContainerIndex > -1
                 ? response.Substring(resultContainerIndex, footerContainerIndex - resultContainerIndex)
                 : response.Substring(resultContainerIndex);

            if (resultContent.Contains(INVALID_CREDS_FORGOT_PASSWORD_TEXT))
            {
                Log.Debug($"ParseForgotPasswordResult: Invalid username text match found in forgot password request for UserName={userName}");
                return ForgotPasswordResult.Invalid;
            }

            if (resultContent.Contains(VALID_CREDS_FORGOT_PASSWORD_TEXT))
            {
                Log.Debug($"ParseForgotPasswordResult: Valid username text match found in forgot password request for UserName={userName}");
                return ForgotPasswordResult.Valid;
            }

            Log.Debug($"ParseForgotPasswordResult: Unable to determine result of forgot password request for UserName={userName}");
            return ForgotPasswordResult.Unknown;
        }

        private static LoginResult ParseValidLoginCredsFromResponse(string response, string userName)
        {
            int resultContainerIndex = response.FindContentIndex("id='container_body_full'");
            int footerContainerIndex = response.FindContentIndex("id='container_footer'");

            string resultContent = footerContainerIndex > -1
                 ? response.Substring(resultContainerIndex, footerContainerIndex - resultContainerIndex)
                 : response.Substring(resultContainerIndex);

            if (resultContent.Contains(INCORRECT_LOGIN_TEXT))
            {
                Log.Debug($"ParseValidLoginCreds: Invalid login text match found in login request for UserName={userName}");
                return LoginResult.InvalidCredientials;
            }

            if (resultContent.Contains(LOCKED_OUT_LOGIN_TEXT))
            {
                Log.Debug($"ParseValidLoginCreds: Locked out text found in login request for UserName={userName}");
                return LoginResult.LockedOut;
            }

            int userLinkIndex = response.FindContentIndex("href='index.asp?page=memberinfo&id=");

            if (userLinkIndex == -1)
            {
                Log.Debug($"ParseValidLoginCreds: Unable to verify login request. No user link found, but request passed invalid checks for UserName={userName}");
                return LoginResult.Unknown;
            }

            if (response.ToLower().Contains(userName.ToLower()))
            {
                Log.Debug($"ParseValidLoginCreds: Login creds were found to be valid. User link for UserName={userName} was matched.");
                return LoginResult.ValidCredientials;
            }

            Log.Debug($"ParseValidLoginCreds: Unable to verify login request. User link was found, but it does not contain UserName={userName}");
            return LoginResult.Unknown;
        }

        private static List<DaySchedule> ParseClassScheduleFromResponse(string response)
        {
            string tableLocationStart = "table id='schedule_table'";
            string tableLocationEnd   = "</table>";
            int    tableStartIndex = response.FindContentIndex("table id='schedule_table'");
            int    tableEndIndex;

            bool log = Constants.Rx.LogScheduleResult;

            if (tableStartIndex < 0) { throw new Exception($"Could not find schedule table on page. String: {tableLocationStart} was missing."); }

            string table = response.Substring(tableStartIndex);

            tableEndIndex = table.IndexOf(tableLocationEnd);

            table = table.Substring(0, tableEndIndex);

            var rows = table.Split(new string[] { "<tr" }, StringSplitOptions.RemoveEmptyEntries);

            List<DaySchedule> days = new List<DaySchedule>();
            DaySchedule currDay = null;

            string titleDayStart = "<font color='#FFFFFF'>";
            string titleDayEnd   = "</font>";
            string timeStart = "<nobr>";
            string timeEnd   = "</nobr>";
            string titleStart = "<b>";
            string titleEnd   = "</b>";
            string classPieceEnd    = "</font>";
            string classPieceStart1 = "<font color='#646464'>";
            string classPieceStart2 = classPieceStart1.Replace("'", "\"");
            string classPieceStart3 = classPieceStart1.Replace("'", "");
            var    classPieces = new List<string> { classPieceStart1, classPieceStart2, classPieceStart3, classPieceEnd };

            for (var i = 1; i < rows.Length; i++)
            {
                string row = rows[i];

                // is header row?
                if (row.Contains("bgcolor='#000000'") || row.Contains("bgcolor=\"#000000\"") || row.Contains("bgcolor=#000000"))
                {
                    string rowContent = row.FindContentSubstring(titleDayStart);

                    int titleEndIndex = rowContent.IndexOf(titleDayEnd);

                    if (log) { Log.Debug($"row: {rowContent}, titleEndIndex={titleEndIndex}"); }

                    rowContent = rowContent.Substring(0, titleEndIndex);

                    if (log) { Log.Debug($"date => {rowContent}"); }

                    DateTime date = DateTime.ParseExact(rowContent, "MMM d,yyyy", CultureInfo.InvariantCulture);

                    currDay = new DaySchedule
                    {
                        Date = date.DateKey(),
                        Day  = date.DayOfWeek
                    };

                    days.Add(currDay);
                }
                // is class row?
                else
                {
                    // class row is without a header? can't determine which day it belongs to if there's no header
                    // for the class or we couldn't parse the header row.
                    if (currDay == null)
                    {
                        continue;
                    }
                    
                    classPieces.ForEach(piece => row = row.Replace(piece, ""));

                    if (log) { Log.Debug($"row => {row}"); }

                    int timeStartIndex = row.IndexOf(timeStart) + timeStart.Length;
                    int timeEndIndex   = row.IndexOf(timeEnd);
                    
                    int titleStartIndex = row.IndexOf(titleStart) + titleStart.Length;
                    int titleEndIndex   = row.IndexOf(titleEnd);

                    if (timeStartIndex < timeStart.Length || timeEndIndex < 0 || titleStartIndex < titleStart.Length || titleEndIndex < 0) { continue; }

                    string timeStr = row.Substring(timeStartIndex,  timeEndIndex - timeStartIndex);
                    string title   = row.Substring(titleStartIndex, titleEndIndex - titleStartIndex);
                    
                    DateTime time;

                    if (DateTime.TryParseExact(timeStr, "h:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                    {
                        EventBlock block = new EventBlock();
                        block.Start = time.TimeKey();
                        block.End   = time.AddHours(1).TimeKey();
                        block.Type  = EventType.Event;
                        block.Title = title;

                        currDay.Blocks.Add(block);
                    }
                }
            }

            if (log) { Log.Debug($"result => {Newtonsoft.Json.JsonConvert.SerializeObject(days)}"); }

            return days;
        }

        private static WodSummary ParseWodFromResponse(string response)
        {
            int titleTagIndex = response.FindContentIndex("id='dispTitle'");
            int trackTagIndex = response.FindContentIndex("id='dispDeets'");
            int  descTagIndex = response.FindContentIndex("id='dispDesc'");
            int      endIndex = response.FindContentIndex("id='vote");

            int numWods = response.Split(new string[] { "href='display.asp?wodindex=" }, StringSplitOptions.None).Length;

            string title, trackBy;
            string[] contents;

            string titleContent = response.Substring(titleTagIndex, trackTagIndex - titleTagIndex);
            string trackContent = response.Substring(trackTagIndex,  descTagIndex - trackTagIndex);
            string  descContent = response.Substring( descTagIndex,      endIndex - descTagIndex);

            int titleLinkStartIndex = titleContent.IndexOf("<a href=");
            int titleLinkEndIndex   = titleContent.IndexOf("</a");

            titleContent = titleContent.Substring(titleLinkStartIndex, titleLinkEndIndex - titleLinkStartIndex);

            titleLinkStartIndex = titleContent.IndexOf(">") + (">").Length;

            title = titleContent.Substring(titleLinkStartIndex);

            int trackContentStartIndex = trackContent.IndexOf("<b>") + ("<b>").Length;
            int trackContentEndIndex   = trackContent.IndexOf("</b>");

            trackBy = trackContent.Substring(trackContentStartIndex, trackContentEndIndex - trackContentStartIndex);

            int descContentStartIndex = descContent.IndexOf(">") + (">").Length;
            int descContentEndIndex   = descContent.IndexOf("</font>");

            descContent = descContent.Substring(descContentStartIndex, descContentEndIndex - descContentStartIndex);

            contents = descContent.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            return new WodSummary { Title = title, TrackBy = trackBy, Contents = contents, WodCount = numWods };
        }
    }
}