using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class SchoolGradeLevel
    {
        public int SchoolID { get; set; }
        public short GradeLevelID { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
