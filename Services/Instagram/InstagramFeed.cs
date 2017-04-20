using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NinjaFit.Api.Services.Instagram
{
    public class InstagramFeed
    {
        [JsonProperty(PropertyName = "entry_data")]
        public InstagramData Data { get; set; }
    }

    public class InstagramData
    {
        public List<InstagramPage> ProfilePage { get; set; }
    }

    public class InstagramPage
    {
        public InstagramUser User { get; set; }
    }

    public class InstagramUser
    {
        public string Username { get; set; }
        public string Biography { get; set; }

        public InstagramMedia Media { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string PageId { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string PageName { get; set; }

        [JsonProperty(PropertyName = "profile_pic_url")]
        public string ProfilePicUrl { get; set; }

        [JsonProperty(PropertyName = "followed_by")]
        public FollowData FollowedBy { get; set; }
        public FollowData Follows { get; set; }
    }

    public class FollowData
    {
        public int Count { get; set; }
    }

    public class InstagramMedia
    {
        public int Count { get; set; }
        public List<InstagramMediaNode> Nodes { get; set; }
    }

    public class InstagramMediaNode
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public int Date { get; set; }
        public MediaDimensions Dimensions { get; set; }
        public Comments Comments { get; set; }
        public string Caption { get; set; }
        public Likes Likes { get; set; }

        [JsonProperty(PropertyName = "thumbnail_src")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty(PropertyName = "is_video")]
        public bool IsVideo { get; set; }

        [JsonProperty(PropertyName = "display_src")]
        public string DisplayUrl { get; set; }

        [JsonProperty(PropertyName = "video_views")]
        public int VideoViews { get; set; }
    }

    public class Likes
    {
        public int Count { get; set; }
    }

    public class Comments
    {
        public int Count { get; set; }
    }

    public class MediaDimensions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}