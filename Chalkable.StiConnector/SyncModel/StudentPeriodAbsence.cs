using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentPeriodAbsence
    {
        public int StudentPeriodAbsenceID { get; set; }
        public int AcadSessionID { get; set; }
        public int StudentID { get; set; }
        public DateTime Date { get; set; }
        public int TimeSlotID { get; set; }
        public int SectionID { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonID { get; set; }
        public decimal AbsenceValue { get; set; }
        public string AbsenceCategory { get; set; }
        public string Note { get; set; }
        public int? OccurrenceID { get; set; }
        public bool IsSystemGenerated { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
