using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentDailyAbsence
    {
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public DateTime Date { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonID { get; set; }
        public decimal AbsenceValue { get; set; }
        public string AbsenceCategory { get; set; }
        public string Note { get; set; }
        public int? OccurrenceID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public int? AlertID { get; set; }
    }
}
