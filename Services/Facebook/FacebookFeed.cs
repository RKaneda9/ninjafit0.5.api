using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace NinjaFit.Api.Services.Facebook
{
    public class FacebookFeed
    {
        public List<FacebookPost> Data   { get; set; }
        public FacebookPaging     Paging { get; set; }
    }

    public class FacebookPaging
    {
        public string Previous { get; set; }
        public string Next     { get; set; }
    }

    public class FacebookPost
    {
        public string Id          { get; set; }
        public string Message     { get; set; }
        public string Picture     { get; set; }
        public string Link        { get; set; }
        public string Name        { get; set; }
        public string Description { get; set; }
        public string Icon        { get; set; }
        public string Caption     { get; set; }
        public string Type        { get; set; }

        [JsonProperty(PropertyName = "object_id")]
        public string ObjectId { get; set; }

        [JsonProperty(PropertyName = "created_time")]
        public string CreatedTime { get; set; }

        public      FacebookAuthor  From    { get; set; }
        public List<FacebookAction> Actions { get; set; }
        
    }

    public class FacebookAuthor
    {
        public string Name     { get; set; }
        public string Category { get; set; }
        public string Id       { get; set; }
    }

    public class FacebookAction
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }
}