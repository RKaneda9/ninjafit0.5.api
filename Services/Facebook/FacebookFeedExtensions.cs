using Newtonsoft.Json;
using NinjaFit.Api.Extensions;
using NinjaFit.Api.Support;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace NinjaFit.Api.Services.Facebook
{
    public static class FacebookFeedExtensions
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static Models.FacebookFeed Convert(this FacebookFeed _feed)
        {
            if (_feed == null || _feed.Data == null || _feed.Data.Count < 1) { return null; }

            var feed = new Models.FacebookFeed();

            feed.Page.Id    = Constants.Facebook.PageId;
            feed.Page.Link  = Constants.Facebook.FacebookPageUrl;
            feed.Page.Image = Constants.Facebook.FacebookProfImg;

            foreach (var post in _feed.Data) { feed.AddPost(post); }
            
            return feed;
        }

        public static List<string> GetHashTags(this string content)
        {
            if (string.IsNullOrEmpty(content)) { return new List<string>(); }

            var regex   = new Regex(@"(?<=#)\w+");
            var matches = regex.Matches(content);

            return matches.Cast<Match>().Select(m => m.Value).ToList();
        }

        public static void AddPost(this Models.FacebookFeed feed, FacebookPost post)
        {
            try
            {
                if (post == null) { throw new ArgumentNullException("post", $"FacebookFeed.Add:method, FacebookPost post passed in was null."); }

                string postId  = post.GetPostId();
                string postUrl = post.GetPostUrl();
                
                if (string.IsNullOrEmpty(feed.Page.Name) && post.From != null)
                {
                    feed.Page.Name  = post.From.Name;
                }

                var _post = new Models.FacebookPost();
                _post.Text       = post.Message;
                _post.Action     = postUrl;
                _post.CreatedKey = post.GetCreatedKey();
                _post.HashTags   = post.Message.GetHashTags();

                _post.HashTagPrefixUrl = Constants.Facebook.HashTagPrefix;

                if (post.Type == Models.FacebookPostType.Link.Get())
                {
                    _post.Link = new Models.FacebookPostLink();
                    _post.Link.Type        = Models.FacebookLinkType.Link;
                    _post.Link.Image       = post.Picture;
                    _post.Link.Href        = post.Link;
                    _post.Link.Title       = post.Name;
                    _post.Link.Description = post.Description;
                    _post.Link.Caption     = post.Caption;
                    feed.Posts.Add(_post);
                }
                else if (post.Type == Models.FacebookPostType.Event.Get())
                {
                    _post.Link = new Models.FacebookPostLink();
                    _post.Link.Type        = Models.FacebookLinkType.Event;
                    _post.Link.Image       = post.Picture;
                    _post.Link.Href        = post.Link;
                    _post.Link.Title       = post.Name;
                    _post.Link.Description = post.Description;
                    _post.Link.Caption     = post.Caption;
                    feed.Posts.Add(_post);
                }
                else if (post.Type == Models.FacebookPostType.Video.Get())
                {
                    string imageUrl = post.Picture;

                    if (string.IsNullOrEmpty(post.Name) &&
                        string.IsNullOrEmpty(post.Description) &&
                        string.IsNullOrEmpty(post.Caption))
                    {
                        imageUrl = post.GetFullImageUrlFromObjectId();
                    }

                    _post.Link = new Models.FacebookPostLink();
                    _post.Link.Type        = Models.FacebookLinkType.Video;
                    _post.Link.Image       = imageUrl;
                    _post.Link.Href        = post.Link;
                    _post.Link.Title       = post.Name;
                    _post.Link.Description = post.Description;
                    _post.Link.Caption     = post.Caption;
                    feed.Posts.Add(_post);
                }
                else if (post.Type == Models.FacebookPostType.Photo.Get() && !string.IsNullOrEmpty(post.ObjectId))
                {
                    _post.Image = new Models.FacebookPostImage();
                    _post.Image.Url  = post.GetFullImageUrlFromObjectId();
                    _post.Image.Link = post.Link;
                    feed.Posts.Add(_post);
                }
                else if (post.Type == Models.FacebookPostType.Status.Get())
                {
                    feed.Posts.Add(_post);
                }
                else
                {
                    Log.Info($"ERROR: Not adding Facebook post because it is an unrecognized type: {post.Type}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"There was a problem parsing Facebook Post: {JsonConvert.SerializeObject(post)}");
            }
        }

        public static string GetFullImageUrlFromObjectId(this FacebookPost post)
        {
            return $"{Constants.Facebook.FacebookApiBaseUrl}{post.ObjectId}/picture";
        }

        public static string Get(this Models.FacebookPostType type)
        {
            return Enum.GetName(typeof(Models.FacebookPostType), type).ToLower();
        }

        public static string GetPostId(this FacebookPost post)
        {
            if (!string.IsNullOrEmpty(post.Id))
            {
                var pieces = post.Id.Split('_');

                if (pieces.Length == 2)
                {
                    return pieces[1];
                }
            }

            return string.Empty;
        }

        public static string GetPostUrl(this FacebookPost post)
        {
            var postId = post.GetPostId();

            return !string.IsNullOrEmpty(postId) ? $"{Constants.Facebook.FacebookPageUrl}posts/{postId}" : string.Empty;
        }

        public static string GetCreatedKey(this FacebookPost post)
        {
            DateTime dateTime;
            DateTime today = DateTime.UtcNow;

            if (DateTime.TryParse(post.CreatedTime, out dateTime))
            {
                return $"{dateTime.DateKey()}{dateTime.TimeKey()}";
            }

            return "N/A";
        }
    }
}