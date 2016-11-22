using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StaffContact
    {
        public int StaffID { get; set; }
        public int ContactID { get; set; }
        public short RelationshipID { get; set; }
        public string Description { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
