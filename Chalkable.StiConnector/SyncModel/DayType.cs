using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class DayType
    {
        public int DayTypeID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short Sequence { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
    }
}
