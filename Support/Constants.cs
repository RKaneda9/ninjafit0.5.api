using System.Web.Configuration;

namespace NinjaFit.Api.Support
{
    public class Constants
    {
        public class Validation
        {
            public const int FirstNameLength = 2;
            public const int  LastNameLength = 4;
            public const int   ContentLength = 20;
            public const int     PhoneLength = 10;
        }
        
        public static class Formats
        {
            public const string DateKey = "yyyyMMdd";
            public const string TimeKey = "HHmm";
        }

        public static class Images
        {
            public const int PreviewWidth  = 300;
            public const int PreviewHeight = 300;
        }

        public class Rx
        {
            public static string BaseUrl     => WebConfigurationManager.AppSettings["rx:base:url"];
            public static string LoginExt    => WebConfigurationManager.AppSettings["rx:login:ext"];
            public static string RegisterUrl => $"{BaseUrl}{LoginExt}?page=newmember";

            public static bool LogLoginResult    => WebConfigurationManager.AppSettings["rx:login:logresult"]     == "true";
            public static bool LogForgotPwdesult => WebConfigurationManager.AppSettings["rx:forgotpwd:logresult"] == "true";
            public static bool LogScheduleResult => WebConfigurationManager.AppSettings["rx:schedule:logresult"]  == "true";
            
        }

        public class Facebook
        {
            public static string PageId             => WebConfigurationManager.AppSettings["facebook:pageid"];
            public static string AccessToken        => WebConfigurationManager.AppSettings["facebook:accesstoken"];
            public static string FieldsExt          => WebConfigurationManager.AppSettings["facebook:fields:ext"];
            public static string PostLimit          => WebConfigurationManager.AppSettings["facebook:post:limit"];
            public static string FacebookApiBaseUrl =>  "https://graph.facebook.com/";
            public static string FacebookApiUrl     => $"https://graph.facebook.com/{PageId}/";
            public static string FacebookPageUrl    => $"https://www.facebook.com/{PageId}/";
            public static string FacebookProfImg    => $"{FacebookApiUrl}picture";
            public static string HashTagPrefix      =>  "https://www.facebook.com/hashtag/";
        }

        public class Instagram
        {
            public static string PageUrl =>           WebConfigurationManager.AppSettings["instagram:pageurl"];
            public static int MediaLimit => int.Parse(WebConfigurationManager.AppSettings["instagram:media:limit"] ?? "12");
        }

        public class Wod
        {
            public static string ExtUrl => WebConfigurationManager.AppSettings["wod:ext:url"];
        }

        public class App
        {
            public static string BaseUrl  => WebConfigurationManager.AppSettings["app:base:url"];
            public static string LoginExt => WebConfigurationManager.AppSettings["app:login:ext"];
        }

        public class Schedule
        {
            public static string StartTime => WebConfigurationManager.AppSettings["schedule:day:start"];
            public static string   EndTime => WebConfigurationManager.AppSettings["schedule:day:end"];
            public static string  RxExtUrl => WebConfigurationManager.AppSettings["rx:class:ext"];
        }

        public class Login
        {
            public static bool LogCreds => WebConfigurationManager.AppSettings["login:logcreds"] == "true";
        }
    }
}