using System;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StiPeriodAbsence
    {
        // For StudentCheckInOut, they wont need AdministratorId, OccurrenceId, RowVersion, DistrictGuid, LevellsOther
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonId { get; set; }
        public decimal AbsenceValue { get; set; }
        public int AcadSessionId { get; set; }
        public DateTime Date { get; set; }
        public bool IsSystemGenerated { get; set; }
        public string Note { get; set; }
        public int TimeSlotId { get; set; }
        public int SectionId { get; set; }
        public int StudentId { get; set; }
        public int StudentPeriodAbsenceId { get; set; }     
    }
}
