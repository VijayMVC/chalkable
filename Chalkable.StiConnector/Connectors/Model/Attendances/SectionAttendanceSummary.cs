using System;
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class SectionAttendanceSummary
    {
        public IEnumerable<DailySectionAbsenceSummary> Days { get; set; }
        public int SectionId { get; set; }
        public IEnumerable<StudentSectionAbsenceSummary> Students { get; set; }
    }
}
