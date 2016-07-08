namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class AttendanceCalendarDay
    {
        public int AttendanceCalendarID { get; set; }
        public DateTime Date { get; set; }
        public int AcadSessionID { get; set; }
        public decimal? Portion { get; set; }
        public int? AllDayValue { get; set; }
        public int? HalfDayValue { get; set; }
        public int? TardyValue { get; set; }
        public bool InSchool { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public decimal Fte { get; set; }
    }
}
