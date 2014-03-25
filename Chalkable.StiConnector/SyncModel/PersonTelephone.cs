using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class PersonTelephone
    {
        public int PersonID { get; set; }
        public string TelephoneNumber { get; set; }
        public string AreaCode { get; set; }
        public string LocalNumber { get; set; }
        public string Extension { get; set; }
        public string FormattedTelephoneNumber { get; set; }
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
