namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public DateTime ReportedTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndTime { get; set; }
        public short ActionID { get; set; }
        public string ActionClass { get; set; }
        public short ReasonID { get; set; }
        public string Note { get; set; }
        public Guid EnrollmentGuid { get; set; }
        public Guid DistrictGuid { get; set; }
        public string DestinationDistrict { get; set; }
        public string DestinationSchool { get; set; }
    
        public virtual AcadSession AcadSession { get; set; }
        public virtual Student Student { get; set; }
    }
}
