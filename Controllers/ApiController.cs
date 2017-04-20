using NinjaFit.Api.Models;
using NLog;
using System;

namespace NinjaFit.Api.Controllers
{
    public class ApiController : System.Web.Http.ApiController
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected ApiResponse Valid<T>(T data)
        {
            return new ApiResponse(true, data);
        }

        protected ApiResponse Valid()
        {
            return new ApiResponse(true, true);
        }

        protected ApiResponse Invalid(Exception ex)
        {
            return new ApiResponse(ex.Message);
        }

        protected ApiResponse Invalid(string message)
        {
            return new ApiResponse(message);
        }
    }
}