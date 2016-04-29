using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class StaffSchool : SyncModel
    {
        public int StaffID { get; set; }
        public int SchoolID { get; set; }
        public Guid DistrictGuid { get; set; }
        public Guid RowVersion { get; set; }

        public override int DefaultOrder => 9;
    }
}
