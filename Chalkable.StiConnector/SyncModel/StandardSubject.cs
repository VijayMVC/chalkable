using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StandardSubject
    {
        public int StandardSubjectID { get; set; }
        public int? AssessmentSubjectID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short? AdoptionYear { get; set; }
        public bool IsActive { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
