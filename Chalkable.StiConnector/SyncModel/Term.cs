using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Term
    {
        public int TermID { get; set; }
        public int AcadSessionID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StateCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public bool AreTranscriptsPosted { get; set; }
        public string ConnectEduTerm { get; set; }
        public byte Sequence { get; set; }
        public Guid RowVersion { get; set; }
        public Guid DistrictGuid { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
    }
}
