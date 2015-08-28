namespace Chalkable.StiConnector.SyncModel
{
    public class ContactRelationship : SyncModel
    {
        public int ContactRelationshipID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ReceivesMailings { get; set; }
        public bool CanPickUp { get; set; }
        public bool IsFamilyMember { get; set; }
        public bool IsCustodian { get; set; }
        public bool IsEmergencyContact { get; set; }
        public string StateCode { get; set; }
        public string SIFCode { get; set; }
        public string NCESCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public override int DefaultOrder => 47;
    }
}
