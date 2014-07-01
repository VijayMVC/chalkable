using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class SectionStaff
    {
        public int SectionID { get; set; }
        public int StaffID { get; set; }
        public bool IsHighlyQualified { get; set; }
        public bool IsCertified { get; set; }
        public bool IsPrimary { get; set; }
        public short? StaffRoleID { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
