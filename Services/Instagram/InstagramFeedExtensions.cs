using NinjaFit.Api.Extensions;
using NinjaFit.Api.Support;
using NLog;
using System;
using System.Globalization;
using System.Linq;

namespace NinjaFit.Api.Services.Instagram
{
    public static class InstagramFeedExtensions
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static string GetUploadedText(this InstagramMediaNode media)
        {
            DateTime dateTime = FromUnixTime(media.Date);
            DateTime today    = DateTime.UtcNow;

            string time = dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture).ToLower();

            if (dateTime.IsToday())
            {
                return $"Today, {time}";
            }

            if (dateTime.IsYesterday())
            {
                return $"Yesterday, {time}";
            }

            return $"{dateTime.ToString("MMMM")} {dateTime.GetDayOfMonthText()}, {time}";
        }

        public static string GetMediaLinkUrl(this InstagramMediaNode media, string userName)
        {
            return $"https://www.instagram.com/p/{media.Code}/?taken-by={userName}";
        }

        public static Models.InstagramFeed Convert(this InstagramFeed feed)
        {
            if (feed.Data == null)
            {
                throw new Exception("Could not convert Instagram feed. feed.Data was null.");
            }

            if (feed.Data.ProfilePage == null || feed.Data.ProfilePage.Count == 0)
            {
                throw new Exception("Could not convert Instagram feed. feed.Data.ProfilePage was null.");
            }

            if (feed.Data.ProfilePage[0].User == null)
            {
                throw new Exception("Could not convert Instagram feed. feed.Data.ProfilePage[0].User was null.");
            }

            var page = feed.Data.ProfilePage[0].User;

            var response = new Models.InstagramFeed();
            response.Meta.PageId     = page.PageId;
            response.Meta.PageName   = page.PageName;
            response.Meta.UserName   = page.Username;
            response.Meta.Bio        = page.Biography;
            response.Meta.FollowedBy = page.FollowedBy?.Count ?? 0;
            response.Meta.Following  = page.Follows   ?.Count ?? 0;
            response.Meta.PostCount  = page.Media.Count;
            response.Meta.ProfilePic = page.ProfilePicUrl;

            foreach (var media in page.Media.Nodes.Take(Constants.Instagram.MediaLimit))
            {
                try
                {
                    var converted = new Models.InstagramMedia();
                    converted.MediaId          = media.Id;
                    converted.Caption          = media.Caption;
                    converted.ThumbnailUrl     = media.ThumbnailUrl;
                    converted.DisplayUrl       = media.DisplayUrl;
                    converted.LinkUrl          = media.GetMediaLinkUrl(page.Username);
                    converted.IsVideo          = media.IsVideo;
                    converted.VideoViews       = media.VideoViews;
                    converted.CommentCount     = media.Comments?.Count ?? 0;
                    converted.LikeCount        = media.Likes   ?.Count ?? 0;
                    converted.UploadedTimeText = media.GetUploadedText();

                    response.Media.Add(converted);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            return response;
        }
    }
}