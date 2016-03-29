using System;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StiDailyAbsence
    {
        // For StudentDailyAbsence, they won't need OccurrenceId, RowVersion, DistrictGuid, or AlertId               
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonId { get; set; }
        public decimal AbsenceValue { get; set; }
        public int AcadSessionId { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentId { get; set; }
    }
}
