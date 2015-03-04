using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class AbsenceLevelReason
    {
        public int AbsenceLevelReasonID { get; set; }
        public string AbsenceLevel { get; set; }
        public short AbsenceReasonID { get; set; }
        public bool IsDefaultReason { get; set; }
        public Guid DistrictGuid { get; set; }
    }
}
