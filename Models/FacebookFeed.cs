using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace NinjaFit.Api.Models
{
    public class FacebookFeed
    {
        public FacebookPageMeta   Page  { get; set; }
        public List<FacebookPost> Posts { get; set; }

        public FacebookFeed()
        {
            Page  = new FacebookPageMeta();
            Posts = new List<FacebookPost>();
        }
    }

    public class FacebookPost
    {
        public string Text   { get; set; }
        public string Action { get; set; }

        public string CreatedKey { get; set; }

        public List<string> HashTags { get; set; }

        public string HashTagPrefixUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FacebookPostType Type { get; set; }

        public FacebookPostLink  Link  { get; set; }
        public FacebookPostImage Image { get; set; }

        public FacebookPost()
        {
            HashTags = new List<string>();
        }
    }

    public class FacebookPageMeta
    {
        public string Id    { get; set; }
        public string Image { get; set; }
        public string Link  { get; set; }
        public string Name  { get; set; }
    }
    
    public class FacebookPostImage
    {
        public string Url  { get; set; }
        public string Link { get; set; }
    }

    public enum FacebookPostType
    {
        Unknown,
        Link,
        Video,
        Photo,
        Event,
        Status
    }

    public enum FacebookLinkType
    {
        Video,
        Link,
        Event
    }

    public class FacebookPostLink
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FacebookLinkType Type { get; set; }

        public string Image       { get; set; }
        public string Title       { get; set; }
        public string Href        { get; set; }
        public string Description { get; set; }
        public string Caption     { get; set; }
    }
}