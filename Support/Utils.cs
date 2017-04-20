using NinjaFit.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NinjaFit.Api.Support
{
    public class Utils
    {
        private static DateTime Now { get { return DateTime.UtcNow; } }
        
        public static int    NowDateKey { get { return Now.DateKey(); } }
        public static string NowTimeKey { get { return Now.TimeKey(); } }

        public static DateTime ToDate(int dateKey)
        {
            return DateTime.ParseExact(dateKey.ToString(), Constants.Formats.DateKey, CultureInfo.InvariantCulture);
        }

        #region Validation

        public static bool IsValidDateKey(int dateKey)
        {
            DateTime date;
            string dateKeyStr = dateKey.ToString();

            if (string.IsNullOrEmpty(dateKeyStr)) { return false; }

            if (dateKeyStr.Length < Constants.Formats.DateKey.Length) { return false; }

            return DateTime.TryParseExact(dateKeyStr, Constants.Formats.DateKey, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        #endregion
    }
}