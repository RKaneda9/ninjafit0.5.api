using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NinjaFit.Api.Models
{
    public class ApiResponse
    {
        public bool   IsValid { get; set; }
        public string Message { get; set; }
        public object Data    { get; set; }

        [JsonConstructor]
        public ApiResponse() { }

        public ApiResponse(string message)
        {
            Message = message ?? "There was an error processing your request.";
        }

        public ApiResponse(bool isValid = false, object data = null)
        {
            IsValid = isValid;
            Message = "There was an error processing your request.";
            Data    = data;
        }

        public static ApiResponse Valid<T>(T data)
        {
            return new ApiResponse(true, data);
        }

        public static ApiResponse Valid()
        {
            return new ApiResponse(true, true);
        }

        public static ApiResponse Invalid(Exception ex)
        {
            return new ApiResponse(ex.Message);
        }

        public static ApiResponse Invalid(string message)
        {
            return new ApiResponse(message);
        }
    }
}