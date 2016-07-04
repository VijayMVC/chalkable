using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.GradingViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceSummaryViewData : StudentProfileViewData
    {
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData> Absences { get; set; }
        public HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData> Lates { get; set; }
        public HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData> Presents { get; set; }

        protected StudentAttendanceSummaryViewData(Student student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions) : base(student, customAlerts, healthConditions)
        {
        }



        public static StudentAttendanceSummaryViewData Create(StudentAttendanceSummary attendanceSummary, GradingPeriod currentGradingPeriod, IList<GradingPeriod> gradingPeriods
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions)
        {
            var res = new StudentAttendanceSummaryViewData(attendanceSummary.Student, customAlerts, healthConditions)
                {
                    GradingPeriods = GradingPeriodViewData.Create(gradingPeriods)
                };
            if (currentGradingPeriod != null)
                res.CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod);
            
            decimal posibleDailyAttendanceCount = 0, dailyLates = 0, dailyAbsents = 0, dailyPresents = 0;
            if (attendanceSummary.DailyAttendanceSummary != null)
            {
                posibleDailyAttendanceCount = attendanceSummary.DailyAttendanceSummary.PosibleAttendanceCount;
                dailyLates = attendanceSummary.DailyAttendanceSummary.Tardies ?? 0;
                dailyAbsents = attendanceSummary.DailyAttendanceSummary.Absences ?? 0;
                dailyPresents = attendanceSummary.DailyAttendanceSummary.Presents ?? 0;
            }
            res.Absences = PrepareAttendanceBox(dailyAbsents, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Absences, 5);
            res.Lates = PrepareAttendanceBox(dailyLates, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Tardies, 10);
            res.Presents = PrepareAttendanceBox(dailyPresents, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Presents, 0, true);
            return res;
        }

        private static HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData> PrepareAttendanceBox(decimal? dailyAttIssuesCount, decimal posibleDailyAttCount
            , IList<StudentClassAttendanceSummary> classAttendanceSummaries, Func<StudentClassAttendanceSummary, decimal?> getAttendanceIssuesCount, int persents, bool isPresentBox = false)
        {
            var res = new HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData>();
            decimal totalAbsences = 0, posibleAbsent = posibleDailyAttCount;
            if (dailyAttIssuesCount.HasValue)
                totalAbsences += dailyAttIssuesCount.Value;

            classAttendanceSummaries =
                classAttendanceSummaries.Where(
                    x => (getAttendanceIssuesCount(x).HasValue ? (int) getAttendanceIssuesCount(x).Value : 0) > 0)
                    .ToList();
            res.Hover = classAttendanceSummaries
                .Select(x => new StudentAttendnaceHoverBoxItemViewData
                {
                    AttendanceCount = getAttendanceIssuesCount(x).HasValue ? (int)getAttendanceIssuesCount(x).Value : 0,
                    ClassName = x.Class.Name
                }).ToList();
            res.Title = totalAbsences.ToString();
            res.IsPassing = isPresentBox || (posibleAbsent > 0 && (totalAbsences * 100) / posibleAbsent < persents);
            return res;
        }
    }

    public class StudentAttendnaceHoverBoxItemViewData
    {
        public int AttendanceCount { get; set; }
        public string ClassName { get; set; }
    }
}