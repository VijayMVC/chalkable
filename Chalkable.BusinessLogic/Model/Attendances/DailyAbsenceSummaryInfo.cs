using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    //TODO: duplicate to StudentDailyAttendnaceSummary ... invastiage and delete this useless class 
    public class DailyAbsenceSummaryInfo : SimpleAttendanceSummary
    {
        public int StudentId { get; set; }

        public static DailyAbsenceSummaryInfo Create(StudentDailyAbsenceSummary dailyAbsenceSummary)
        {
            return new DailyAbsenceSummaryInfo
            {
                Absences = dailyAbsenceSummary.Absences,
                Tardies = dailyAbsenceSummary.Tardies,
                Presents = dailyAbsenceSummary.Presents,
                StudentId = dailyAbsenceSummary.StudentId
            };
        }
    }
}
