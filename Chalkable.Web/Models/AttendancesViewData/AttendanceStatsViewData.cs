using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class AttendanceStatsViewData
    {
        public int AbsencesCount { get; set; }
        public int ExcusedCount { get; set; }
        public int LatesCount { get; set; }
        public string Summary { get; set; }
        public DateTime? Date { get; set; }

        public static AttendanceStatsViewData Create(IList<ClassAttendanceDetails> attendances, DateTime? date, string summary)
        {
            return new AttendanceStatsViewData
            {
                AbsencesCount = attendances.Count(x=>x.IsAbsent),
                ExcusedCount = attendances.Count(x => x.IsExcused),
                LatesCount = attendances.Count(x => x.IsLate),
                Summary = summary,
                Date = date
            };
        }

        public static AttendanceStatsViewData Create(IList<AttendanceTotalPerType> attTotalPerType, DateTime? date, string summary)
        {
            var res = new AttendanceStatsViewData {Date = date, Summary = summary};
            if (attTotalPerType != null && attTotalPerType.Count > 0)
            {
                var dic = attTotalPerType.GroupBy(x => x.Level).ToDictionary(x => x.Key, x => x.Sum(y => y.Total));
                res.AbsencesCount = 0;
                foreach (var level in ClassAttendance.AbsentLevels)
                {
                    res.AbsencesCount = dic.ContainsKey(level) ? dic[level] : 0;
                }
                res.LatesCount = 0;
                foreach (var level in ClassAttendance.LateLevels)
                {
                    res.AbsencesCount = dic.ContainsKey(level) ? dic[level] : 0;
                }
                res.ExcusedCount = 0;//TODO: what to do?
            }
            return res;
        }

        public static IList<AttendanceStatsViewData> BuildStatsPerDate(IDictionary<DateTime, IList<AttendanceTotalPerType>> attsTotalPerDate
            , IList<Date> dates, string dateFormat)
        {
            var res = new List<AttendanceStatsViewData>();
            foreach (var date in dates)
            {
                var attsTotal = attsTotalPerDate.ContainsKey(date.Day) ? attsTotalPerDate[date.Day] : null;
                res.Add(Create(attsTotal, date.Day, date.Day.ToString(dateFormat)));
            }
            return res;
        }

        public static IList<AttendanceStatsViewData> BuildStatsPerPeriod(IDictionary<int, IList<AttendanceTotalPerType>> attsTotalPerPeriod
            , int toPeriodOrder)
        {
            var res = new List<AttendanceStatsViewData>();
            for (var i = 1; i <= toPeriodOrder; i++)
            {
                var attsTotal = attsTotalPerPeriod.ContainsKey(i) ? attsTotalPerPeriod[i] : null;
                res.Add(Create(attsTotal, null, StringTools.NumberToStr(i)));
            }
            return res;
        }
    }
}