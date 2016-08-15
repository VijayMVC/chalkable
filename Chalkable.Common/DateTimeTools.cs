using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Chalkable.Common
{
    public static class DateTimeTools
    {
        private static Dictionary<string, TimeZoneInfo> timeZones;
        static DateTimeTools()
        {
            var tzCollection = TimeZoneInfo.GetSystemTimeZones();
            timeZones = tzCollection.ToDictionary(x => x.Id);
        }


        public static DateTime Min(DateTime a, DateTime b)
        {
            return a < b ? a : b;
        }

        public static DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }
        
        public static TimeZoneInfo GetById(string id)
        {
            return timeZones[id];
        }

        public static IEnumerable<TimeZoneInfo> GetAll()
        {
            return timeZones.Values;
        }

        public static DateTime ConvertFromUtc(this DateTime dateTime, string timeZoneId)
        {
            var tz = GetById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, tz);
        }

        public static DateTime ConvertToUtc(this DateTime dateTime, string timeZoneId)
        {
            var tz = GetById(timeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, tz);
        }

        public static string WindowsToIana(string windowsZoneId)
        {
            if (windowsZoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
                return "Etc/UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(windowsZoneId);
            var timeZoneId = tzdbSource.MapTimeZoneId(tzi);

            return tzdbSource.CanonicalIdMap.ContainsKey(timeZoneId) ? tzdbSource.CanonicalIdMap[timeZoneId] : "Etc/UTC";
        }

        public static DateTime? ParseExactNullable(string s, string dateFormat = Constants.DATE_FORMAT)
        {
            return string.IsNullOrWhiteSpace(s) ? (DateTime?)null : DateTime.ParseExact(s, dateFormat, CultureInfo.InvariantCulture);
        }
    }
}
