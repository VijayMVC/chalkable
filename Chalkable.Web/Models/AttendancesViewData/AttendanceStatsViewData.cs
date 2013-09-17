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

        public static IList<AttendanceStatsViewData> BuildStatsPerDate(IList<ClassAttendanceDetails> attendances, string dateFormat)
        {
            return attendances.GroupBy(x => x.Date).Select(x => Create(x.ToList(), x.Key, x.Key.ToString(dateFormat))).ToList();
        }

        public static IList<AttendanceStatsViewData> BuildStatsPerPeriod(IList<ClassAttendanceDetails> attendances)
        {
            var periodRecords = attendances.GroupBy(x => x.ClassPeriod.Period.Order).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToList());
            return periodRecords.Select(periodRecord => Create(periodRecord.Value, null, StringTools.NumberToStr(periodRecord.Key))).ToList();
        }
    }
}