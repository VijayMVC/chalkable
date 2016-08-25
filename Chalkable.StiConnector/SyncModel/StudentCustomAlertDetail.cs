using System;

namespace Chalkable.StiConnector.SyncModel
{
    public class StudentCustomAlertDetail : SyncModel
    {
        public int StudentCustomAlertDetailID { get; set; }
        public int CustomAlertDetailID { get; set; }
        public int StudentID { get; set; }
        public int AcadSessionID { get; set; }
        public string AlertText { get; set; }
        public string CurrentValue { get; set; }
        public override int DefaultOrder => 50;

    }
}
