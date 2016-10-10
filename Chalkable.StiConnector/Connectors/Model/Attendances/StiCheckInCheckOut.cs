using System;

namespace Chalkable.StiConnector.Connectors.Model.Attendances
{
    public class StiCheckInCheckOut
    {
        // For StudentCheckInOut, they wont need AdministratorId, OccurrenceId, RowVersion, DistrictGuid, LevellsOther
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonId { get; set; }
        public int AcadSessionId { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int StudentCheckInOutId { get; set; }
        public int StudentId { get; set; }
        public short Time { get; set; }
        public int? TimeSlotId { get; set; }  
    }
}
