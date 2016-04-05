using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StudentGradeLevel
    {
        public int StudentGradeLevelID { get; set; }
        public int StudentID { get; set; }
        public short GradeLevelID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndTime { get; set; }
        public string Reason { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public bool IsSystem { get; set; }
    }
}
