using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentAcadSession
    {
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public int SchoolID { get; set; }
        public int? AttendanceCalendarID { get; set; }
        public short? GradeLevelID { get; set; }
        public string GradeLevelName { get; set; }
        public string CurrentEnrollmentStatus { get; set; }
        public string LastEnrollmentActionClass { get; set; }
        public int? HomeroomID { get; set; }
        public string HomeroomName { get; set; }
        public short? LunchProgramID { get; set; }
        public string LunchProgramName { get; set; }
        public short? TransportMethodID { get; set; }
        public string TransportMethodName { get; set; }
        public decimal ReportedFTE { get; set; }
        public Guid RowVersion { get; set; }
        public Guid EnrollmentRowVersion { get; set; }
        public Guid HomeroomRowVersion { get; set; }
        public Guid SchedulingRowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsRetained { get; set; }
    }
}
