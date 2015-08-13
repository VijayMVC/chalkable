using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class ClassAttendanceSummary : SimpleAttendanceSummary
    {
        public int ClassId { get; set; }

        public static ClassAttendanceSummary Create(SectionAbsenceSummary sectionAbsenceSummary)
        {
            return new ClassAttendanceSummary
            {
                ClassId = sectionAbsenceSummary.SectionId,
                Absences = sectionAbsenceSummary.Absences,
                Tardies = sectionAbsenceSummary.Tardies,
                Presents = sectionAbsenceSummary.Presents
            };
        }
    }
}
