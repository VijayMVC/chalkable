using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class BellSchedule
    {
        public int BellScheduleID { get; set; }
        public int AcadSessionID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short TotalMinutes { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public bool UseStartEndTime { get; set; }
    }
}
