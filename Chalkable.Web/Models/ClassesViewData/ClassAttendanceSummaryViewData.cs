using System.Collections.Generic;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassAttendanceSummaryViewData
    {
        public int ClassId { get; set; }
        public int DatePeriodType { get; set; }
        public IList<DailyStatsViewData> Absences { get; set; } 
        public IList<DailyStatsViewData> Lates { get; set; }
        public IList<DailyStatsViewData> Presents { get; set; }

        public static ClassAttendanceSummaryViewData Create(int classId, ClassLogic.DatePeriodTypeEnum datePeriodType
            , IList<DailyStatsViewData> dailyAbsences, IList<DailyStatsViewData> dailyLates, IList<DailyStatsViewData> dailyPresents)
        {
            return new ClassAttendanceSummaryViewData
            {
                ClassId = classId,
                DatePeriodType = (int) datePeriodType,
                Absences = dailyAbsences,
                Lates = dailyLates,
                Presents = dailyPresents
            };
        }
    }
}