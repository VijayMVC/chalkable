using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
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
