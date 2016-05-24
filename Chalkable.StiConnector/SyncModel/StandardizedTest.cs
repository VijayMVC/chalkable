

namespace Chalkable.StiConnector.SyncModel
{
    public class StandardizedTest : SyncModel
    {
        public int StandardizedTestID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? GradeLevelID { get; set; }
        public string StateCode { get; set; }
        public bool DisplayOnTranscript { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public override int DefaultOrder => 52;
    }

    public class StandardizedTestComponent : SyncModel
    {
        public int StandardizedTestComponentID { get; set; }
        public int StandardizedTestID { get; set; }
        public string Name { get; set; }
        public bool DisplayOnTranscript { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public override int DefaultOrder => 53;
    }

    public class StandardizedTestScoreType : SyncModel
    {
        public int StandardizedTestScoreTypeID { get; set; }
        public int StandardizedTestID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public override int DefaultOrder => 54;
    }
}
