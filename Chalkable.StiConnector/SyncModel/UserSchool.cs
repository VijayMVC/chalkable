using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class UserSchool
    {
        public int UserID { get; set; }
        public int SchoolID { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
