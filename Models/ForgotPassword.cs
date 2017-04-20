
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NinjaFit.Api.Models
{
    public class ForgotPasswordRequest
    {
        public string UserName { get; set; }
    }

    public class ForgotPasswordResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ForgotPasswordResult Result       { get; set; }
        public string               RedirectLink { get; set; }
    }

    public enum ForgotPasswordResult
    {
        Unknown,
        Empty,
        Invalid,
        Valid
    }
}