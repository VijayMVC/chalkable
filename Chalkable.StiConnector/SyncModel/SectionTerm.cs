using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class SectionTerm
    {
        public int SectionID { get; set; }
        public int TermID { get; set; }
        public bool AwardCredit { get; set; }
        public short? MaximumStudentEnrollment { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
