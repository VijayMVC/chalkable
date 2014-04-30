namespace Chalkable.StiConnector.SyncModel
{
    using System;
    
    public class Address
    {
        public int AddressID { get; set; }
        public string AddressNumber { get; set; }
        public string StreetNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public short? CountyID { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string SubdivisionName { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public short? StateID { get; set; }
        public short? CountryID { get; set; }
    }
}
