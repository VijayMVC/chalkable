using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class FinalGradesViewData
    {
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public GradingPeriodFinalGradeViewData CurrentFinalGrade { get; set; }

        public static FinalGradesViewData Create(IList<GradingPeriod> gradingPeriods)
        {
            var res = new FinalGradesViewData
                {
                    GradingPeriods = GradingPeriodViewData.Create(gradingPeriods),
                };
            return res;
        }
    }

    public class GradingPeriodFinalGradeViewData
    {
        public GradingPeriod GradingPeriod { get; set; }
        public ShortAverageViewData CurrentAverage { get; set; }
        public IList<ShortAverageViewData> Averages { get; set; }
        public IList<StudentFinalGradeViewData> StudentFinalGrades { get; set; }
    }

    public class StudentFinalGradeViewData
    {
        public ShortPersonViewData Student { get; set; }
        public StudentAveragesViewData CurrentStudentAverage { get; set; }
        public IList<StudentAveragesViewData> StudentAverages { get; set; }
        public IList<StudentGradingByTypeStatsViewData> StatsByType { get; set; }
        public StudentFinalAttendanceSummaryViewData Attendance { get; set; }
        public IList<DisciplineTypeSummaryViewData> Disciplines { get; set; } 
    }

    public class StudentFinalAttendanceSummaryViewData
    {
        public TotalAttendanceViewData TotalStudentAttendance { get; set; }
        public TotalAttendanceViewData TotalClassAttendance { get; set; }
    }

    public class TotalAttendanceViewData
    {
        public int LateCount { get; set; }
        public int PrecentCount { get; set; }
        public int AbsentCount { get; set; }
        public int DaysCount { get; set; }
    }

    public class StudentGradingByTypeStatsViewData
    {
        public ClassAnnouncementTypeViewData ClassAnnouncementType { get; set; }
        public IList<StudentGradingStatsViewData> StudentGradingStats { get; set; }

    }
    public class StudentGradingStatsViewData
    {
        public DateTime Date { get; set; }
        public IList<ShortAnnouncementGradeViewData> AnnouncementGrades { get; set; }
        public int Grade { get; set; }
    }
}