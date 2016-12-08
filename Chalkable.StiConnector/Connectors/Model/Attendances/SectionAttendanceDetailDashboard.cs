using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class SectionAttendanceDetailDashboard
    {
        /// <summary>
        /// A list of every period absence for every student enrolled in the section for the time frame (between start and end dates)
        /// </summary>
        public IEnumerable<StiPeriodAbsence> PeriodAbsences { get; set; }
    }
}
