using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentCheckInOut
    {
        public int StudentCheckInOutID { get; set; }
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public DateTime Date { get; set; }
        public short Time { get; set; }
        public int? TimeSlotID { get; set; }
        public string Action { get; set; }
        public int? AdministratorID { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonID { get; set; }
        public string AbsenceCategory { get; set; }
        public string Note { get; set; }
        public int? OccurrenceID { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool LevelIsOther { get; set; }
    }
}
