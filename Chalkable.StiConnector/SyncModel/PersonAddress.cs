using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class PersonAddress
    {
        public int PersonAddressID { get; set; }
        public int PersonID { get; set; }
        public int AddressID { get; set; }
        public string Description { get; set; }
        public bool IsHeadOfHousehold { get; set; }
        public bool IsListed { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
