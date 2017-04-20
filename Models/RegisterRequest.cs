
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NinjaFit.Api.Support;

namespace NinjaFit.Api.Models
{
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        public string     Email { get; set; }
        public string     Phone { get; set; }
        public string  Referral { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public Gender Gender { get; set; }
    }

    public class RegisterResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RegisterResult Result       { get; set; }
        public string         ErrorMsg     { get; set; }
        public string         RedirectLink { get; set; }

        public static RegisterResponse Invalid(string errMsg = "There was a problem with the register request. Please try again.")
        {
            return new RegisterResponse
            {
                Result       = RegisterResult.Error,
                ErrorMsg     = errMsg,
                RedirectLink = Constants.Rx.RegisterUrl
            };
        }
    }

    public enum RegisterResult
    {
        Unknown,
        Error
    }

    public enum Gender
    {
        Male,
        Female
    }
}