using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StudentAttendanceDetailDashboard
    {
        /// <summary>
        /// A list of every check-in/check-out for the student for the time frame(specific term or year)
        /// </summary>
        public IEnumerable<StiCheckInCheckOut> CheckInCheckOuts { get; set; }

        /// <summary>
        /// A list of every daily absence for the time frame(specific term or year)
        /// </summary>
        public IEnumerable<StiDailyAbsence> DailyAbsences { get; set; }

        /// <summary>
        /// A list of every period absence for each section the student is enrolled for the time frame (specific term or year)
        /// </summary>
        public IEnumerable<StiPeriodAbsence> PeriodAbsences { get; set; }   
    }
}
