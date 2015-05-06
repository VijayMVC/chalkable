using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class ClassAttendanceSummaryInfo
    {
        public int ClassId { get; set; }
        public decimal Absences { get; set; }
        public int Tardies { get; set; }

        public static IList<ClassAttendanceSummaryInfo> Create(IList<StudentSectionAbsenceSummary> absences)
        {
            return absences.Select(x => new ClassAttendanceSummaryInfo
            {
                ClassId = x.SectionId,
                Absences = x.Absences ?? 0,
                Tardies = x.Tardies ?? 0
            }).ToList();
        }
    }
}
