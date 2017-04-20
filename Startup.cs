using System.Collections.Generic;
using Microsoft.Owin;
using Owin;
using System.Web.Configuration;
using System.IdentityModel.Tokens;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using Microsoft.Owin.Cors;
using System.Web.Http.ExceptionHandling;
using NinjaFit.Api.Support;
using NLog;

[assembly: OwinStartup(typeof(NinjaFit.Api.Startup))]

namespace NinjaFit.Api
{
    public partial class Startup
    {
        private Logger Logger = LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            Logger.Debug("Application Started. Configuring...");

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            //ConfigureAmazon(app);

            HttpConfiguration config = new HttpConfiguration();

            var jsonFormatter = config.Formatters.JsonFormatter;
            var settings      = jsonFormatter.SerializerSettings;

            settings.Formatting        = Newtonsoft.Json.Formatting.Indented;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            settings.ContractResolver  = new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { action = "get", id = RouteParameter.Optional }
            );

            // For logging of WebApi unhandled exceptions 
            config.Services.Add(typeof(IExceptionLogger), new ApiExceptionLogger());

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);

            Logger.Debug("Configuration finished...");
        }

        public void ConfigureAmazon(IAppBuilder app)
        {
            string profileName = WebConfigurationManager.AppSettings["AWSProfileName"],
                   accessKeyId = WebConfigurationManager.AppSettings["AWSAccessKey"],
                   secretKey   = WebConfigurationManager.AppSettings["AWSSecretKey"];

            Amazon.Util.ProfileManager.RegisterProfile(profileName, accessKeyId, secretKey);
        }
    }
}
