using NinjaFit.Api.Support;
using System;

namespace NinjaFit.Api.Extensions
{
    public static class DateExtensions
    {
        public static bool IsToday(this DateTime dateTime)
        {
            DateTime now = DateTime.UtcNow;

            return dateTime.Year  == now.Year
                && dateTime.Month == now.Month
                && dateTime.Day   == now.Day;
        }

        public static bool IsYesterday(this DateTime dateTime)
        {
            DateTime now = DateTime.UtcNow;
            
            return dateTime.Year  == now.Year
                && dateTime.Month == now.Month
                && dateTime.Day   == now.Day - 1;
        }

        public static int DateKey(this DateTime date)
        {
            return int.Parse(date.ToString(Constants.Formats.DateKey));
        }

        public static string TimeKey(this DateTime date)
        {
            return date.ToString(Constants.Formats.TimeKey);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime Copy(this DateTime date)
        {
            return date.AddDays(0);
        }

        public static DateTime EndOfWeek(this DateTime date)
        {
            return date.AddDays(6 - (int)date.DayOfWeek);
        }

        public static DateTime StartOfWeek(this DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek);
        }

        public static DateTime PreviousDay(this DateTime date)
        {
            return date.AddDays(-1);
        }

        public static DateTime NextDay(this DateTime date)
        {
            return date.AddDays(1);
        }

        public static string GetDayOfMonthText(this DateTime dateTime)
        {
            switch (dateTime.Day)
            {
                case 1 : return "1st";
                case 2 : return "2nd";
                case 3 : return "3rd";
                case 4 : return "4th";
                case 5 : return "5th";
                case 6 : return "6th";
                case 7 : return "7th";
                case 8 : return "8th";
                case 9 : return "9th";
                case 10: return "10th";
                case 11: return "11th";
                case 12: return "12th";
                case 13: return "13th";
                case 14: return "14th";
                case 15: return "15th";
                case 16: return "16th";
                case 17: return "17th";
                case 18: return "18th";
                case 19: return "19th";
                case 20: return "20th";
                case 21: return "21st";
                case 22: return "22nd";
                case 23: return "23rd";
                case 24: return "24th";
                case 25: return "25th";
                case 26: return "26th";
                case 27: return "27th";
                case 28: return "28th";
                case 29: return "29th";
                case 30: return "30th";
                case 31: return "31st";
            }

            return string.Empty;
        }
    }
}