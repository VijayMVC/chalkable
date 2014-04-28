using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Standard
    {
        public int StandardID { get; set; }
        public int? ParentStandardID { get; set; }
        public int? AssessmentStandardID { get; set; }
        public int? AssessmentParentStandardID { get; set; }
        public int StandardSubjectID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short? LowerGradeLevelID { get; set; }
        public short? UpperGradeLevelID { get; set; }
        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
