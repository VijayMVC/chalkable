namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class AcadSession
    {
        public int AcadSessionID { get; set; }
        public int SchoolID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndTime { get; set; }
        public short AcadYear { get; set; }
        public string SchedulingModel { get; set; }
        public short? DaysInCycle { get; set; }
        public bool IsClosed { get; set; }
        public bool AreStudentsPromoted { get; set; }
        public bool GradeBookRequiresCalculation { get; set; }
        public Guid RowVersion { get; set; }
        public Guid AcadSessionGuid { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsVisibleInHomePortal { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public short? AcadSessionTypeID { get; set; }
    }
}
