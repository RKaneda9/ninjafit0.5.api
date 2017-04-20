using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NinjaFit.Api.Models
{
    public class InstagramFeed
    {
        public      InstagramMeta   Meta  { get; set; }
        public List<InstagramMedia> Media { get; set; }

        public InstagramFeed()
        {
            Meta  = new      InstagramMeta();
            Media = new List<InstagramMedia>();
        }
    }

    public class InstagramMedia
    {
        public string MediaId          { get; set; }
        public string UploadedTimeText { get; set; }
        public string Caption          { get; set; }
        public string ThumbnailUrl     { get; set; }
        public string DisplayUrl       { get; set; }
        public string LinkUrl          { get; set; }
        public bool   IsVideo          { get; set; }
        public int    VideoViews       { get; set; }
        public int    CommentCount     { get; set; }
        public int    LikeCount        { get; set; }
    }

    public class InstagramMeta
    {
        public string PageId     { get; set; }
        public string PageName   { get; set; }
        public string UserName   { get; set; }
        public string Bio        { get; set; }
        public string ProfilePic { get; set; }
        public int    FollowedBy { get; set; }
        public int    Following  { get; set; }
        public int    PostCount  { get; set; }
    }
}