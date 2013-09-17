using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                AbsencesCount = attendances.Count(x => x.Type == AttendanceTypeEnum.Absent),
                ExcusedCount = attendances.Count(x => x.Type == AttendanceTypeEnum.Excused),
                LatesCount = attendances.Count(x => x.Type == AttendanceTypeEnum.Late),
                Summary = summary,
                Date = date
            };
        }

        public static AttendanceStatsViewData Create(IList<AttendanceTotalPerType> attTotalPerType, DateTime? date, string summary)
        {
            var res = new AttendanceStatsViewData {Date = date, Summary = summary};
            if (attTotalPerType != null && attTotalPerType.Count > 0)
            {
                var dic = attTotalPerType.GroupBy(x => x.AttendanceType).ToDictionary(x => x.Key, x => x.Sum(y => y.Total));
                res.AbsencesCount = dic.ContainsKey(AttendanceTypeEnum.Absent) ? dic[AttendanceTypeEnum.Absent] : 0;
                res.ExcusedCount = dic.ContainsKey(AttendanceTypeEnum.Excused) ? dic[AttendanceTypeEnum.Excused] : 0;
                res.LatesCount = dic.ContainsKey(AttendanceTypeEnum.Late) ? dic[AttendanceTypeEnum.Late] : 0;
            }
            return res;
        }

        public static IList<AttendanceStatsViewData> BuildStatsPerDate(IDictionary<DateTime, IList<AttendanceTotalPerType>> attsTotalPerDate
            , IList<DateDetails> dates, string dateFormat)
        {
            var res = new List<AttendanceStatsViewData>();
            foreach (var date in dates)
            {
                var attsTotal = attsTotalPerDate.ContainsKey(date.DateTime) ? attsTotalPerDate[date.DateTime] : null;
                res.Add(Create(attsTotal, date.DateTime, date.DateTime.ToString(dateFormat)));
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