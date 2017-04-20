
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NinjaFit.Api.Models
{
    public class LoginCredientials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Page     { get; set; }
    }

    public class LoginResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LoginResult Result       { get; set; }
        public string      RedirectLink { get; set; }
    }

    public enum LoginResult
    {
        Unknown,
        InvalidCredientials,
        LockedOut,
        ValidCredientials
    }
}