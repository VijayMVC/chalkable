using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class PersonEmail : SyncModel
    {
        public int PersonID { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
        public Guid DistrictGuid { get; set; }

        public override int DefaultOrder => 10;
    }
}
