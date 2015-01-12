namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class CalendarDay
    {
        public int AcadSessionID { get; set; }
        public DateTime Date { get; set; }
        public int? DayTypeID { get; set; }
        public int? BellScheduleID { get; set; }
        public string Description { get; set; }
        public decimal FractionOfDay { get; set; }
        public bool InCalendar { get; set; }
        public bool InSchool { get; set; }
        public bool IsInstructional { get; set; }
        public bool IsNonMembershipDay { get; set; }
    }
}
