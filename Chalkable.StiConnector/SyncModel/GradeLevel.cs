using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class GradeLevel
    {
        public short GradeLevelID { get; set; }
        public string Name { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public string Description { get; set; }
        public bool IsGradUponPromo { get; set; }
        public short Sequence { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public byte? FreezeAfterTermSeq { get; set; }
    }
}
