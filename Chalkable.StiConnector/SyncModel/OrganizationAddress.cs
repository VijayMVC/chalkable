using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class OrganizationAddress
    {
        public int OrganizationAddressID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public short? CountyID { get; set; }
        public Guid DistrictGuid { get; set; }
        public short? StateID { get; set; }
        public short? CountryID { get; set; }
    }
}
