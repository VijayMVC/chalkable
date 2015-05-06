using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceSummaryViewData : StudentViewData
    {
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public StudentAttendanceHoverBox Absences { get; set; }
        public StudentAttendanceHoverBox Lates { get; set; }
        public StudentAttendanceHoverBox Presents { get; set; }

        protected StudentAttendanceSummaryViewData(StudentDetails student) : base(student)
        {
        }

        public static StudentAttendanceSummaryViewData Create(StudentAttendanceSummary attendanceSummary, GradingPeriod currentGradingPeriod, IList<GradingPeriod> gradingPeriods)
        {
            var res = new StudentAttendanceSummaryViewData(attendanceSummary.Student)
                {
                    GradingPeriods = GradingPeriodViewData.Create(gradingPeriods)
                };
            if (currentGradingPeriod != null)
                res.CurrentGradingPeriod = GradingPeriodViewData.Create(currentGradingPeriod);
            
            decimal posibleDailyAttendanceCount = 0, dailyLates = 0, dailyAbsents = 0, dailyPresents = 0;
            if (attendanceSummary.DailyAttendanceSummary != null)
            {
                posibleDailyAttendanceCount = attendanceSummary.DailyAttendanceSummary.TotalAttendanceCount;
                dailyLates = attendanceSummary.DailyAttendanceSummary.Tardies ?? 0;
                dailyAbsents = attendanceSummary.DailyAttendanceSummary.Absences ?? 0;
                dailyPresents = attendanceSummary.DailyAttendanceSummary.Presents ?? 0;
            }
            res.Absences = PrepareAttendanceBox(dailyAbsents, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Absences);
            res.Lates = PrepareAttendanceBox(dailyLates, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Tardies);
            res.Presents = PrepareAttendanceBox(dailyPresents, posibleDailyAttendanceCount, attendanceSummary.ClassAttendanceSummaries, x => x.Presents, true);
            return res;
        }
        
        private static StudentAttendanceHoverBox PrepareAttendanceBox(decimal? dailyAttIssuesCount, decimal posibleDailyAttCount
            , IList<StudentClassAttendanceSummary> classAttendanceSummaries, Func<StudentClassAttendanceSummary, decimal?> getAttendanceIssuesCount, bool isPresentBox = false)
        {
            var res = new StudentAttendanceHoverBox();
            decimal totalAbsences = 0, posibleAbsent = posibleDailyAttCount;
            if (dailyAttIssuesCount.HasValue)
                totalAbsences += dailyAttIssuesCount.Value;
            
            res.Hover = classAttendanceSummaries
                .Select(x => new StudentAttendnaceHoverBoxItemViewData
                {
                    AttendnaceCount = getAttendanceIssuesCount(x).HasValue ? (int)getAttendanceIssuesCount(x).Value : 0,
                    ClassName = x.Class.Name
                }).ToList();
            if (classAttendanceSummaries.Count > 0)
            {
                totalAbsences += res.Hover.Sum(x => x.AttendnaceCount);
                posibleAbsent += classAttendanceSummaries.Sum(x => x.PosibleAttendanceCount);
            }
            res.Title = totalAbsences.ToString();
            res.IsPassing = isPresentBox || (posibleAbsent > 0 && (totalAbsences*100)/posibleAbsent > 5);
            return res;
        }
    }

    public class StudentAttendanceHoverBox : HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData>
    {
        public bool IsPassing { get; set; }
    }

    public class StudentAttendnaceHoverBoxItemViewData
    {
        public int AttendnaceCount { get; set; }
        public string ClassName { get; set; }
    }
}