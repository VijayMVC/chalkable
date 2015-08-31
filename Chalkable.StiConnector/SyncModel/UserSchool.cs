using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class UserSchool : SyncModel
    {
        public int UserID { get; set; }
        public int SchoolID { get; set; }
        public Guid DistrictGuid { get; set; }

        public override int DefaultOrder => 4;
    }
}
