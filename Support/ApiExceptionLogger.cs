using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Web.Http.ExceptionHandling;

namespace NinjaFit.Api.Support
{
    public class ApiExceptionLogger : ExceptionLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void Log(ExceptionLoggerContext context)
        {
            try
            {

                var route = context.Request?.RequestUri?.ToString();
                var method = context.Request?.Method?.Method;

                var logObject = new JObject
                {
                    {nameof(route), route},
                    {nameof(method), method},
                    {"exception", JObject.FromObject(context.Exception)}
                };

                Logger.Error(JsonConvert.SerializeObject(logObject));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}