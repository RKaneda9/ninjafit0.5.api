using System;
using System.Linq;

namespace NinjaFit.Api.Services.Rx
{
    public static class RxExtensions
    {
        public static int FindContentIndex(this string val, string search)
        {
            string doubleQuotesSearch = search            .Replace("'",  "\"");
            string singleQuotesSearch = search            .Replace("\"", "'");
            string     noQuotesSearch = singleQuotesSearch.Replace("'",  "");

            int doubleQuotesIndex = val.IndexOf(doubleQuotesSearch);
            int singleQuotesIndex = val.IndexOf(singleQuotesSearch);
            int     noQuotesIndex = val.IndexOf(noQuotesSearch);

            return (new int[] { doubleQuotesIndex, singleQuotesIndex, noQuotesIndex }).Max();
        }

        public static string FindContentSubstring(this string val, string search)
        {
            string doubleQuotesSearch = search.Replace("'", "\"");
            
            int doubleQuotesIndex = val.IndexOf(doubleQuotesSearch);

            if (doubleQuotesIndex > -1)
            {
                return val.Substring(doubleQuotesIndex + doubleQuotesSearch.Length);
            }

            string singleQuotesSearch = search.Replace("\"", "'");

            int singleQuotesIndex = val.IndexOf(singleQuotesSearch);

            if (singleQuotesIndex > -1)
            {
                return val.Substring(singleQuotesIndex + singleQuotesSearch.Length);
            }

            string noQuotesSearch = singleQuotesSearch.Replace("'", "");

            int noQuotesIndex = val.IndexOf(noQuotesSearch);

            if (noQuotesIndex > -1)
            {
                return val.Substring(noQuotesIndex + noQuotesSearch.Length);
            }

            return string.Empty;
        }
    }
}