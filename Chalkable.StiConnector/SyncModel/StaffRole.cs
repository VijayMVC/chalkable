using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StaffRole
    {
        public short StaffRoleID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
