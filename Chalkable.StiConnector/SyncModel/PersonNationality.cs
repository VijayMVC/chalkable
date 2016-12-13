using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class PersonNationality : SyncModel
    {
        public int PersonNationalityID { get; set; }
        public int PersonID { get; set; }
        public string Nationality { get; set; }
        public bool IsPrimary { get; set; }
        public Guid DistrictGuid { get; set; }
        public int NationalityID { get; set; }
        public override int DefaultOrder => 6;
    }
}
