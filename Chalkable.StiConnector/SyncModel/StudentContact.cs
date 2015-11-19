namespace Chalkable.StiConnector.SyncModel
{
    public class StudentContact : SyncModel
    {
        public int StudentID { get; set; }
        public int ContactID { get; set; }
        public int RelationshipID { get; set; }
        public string Description { get; set; }
        public bool ReceivesMailings { get; set; }
        public bool CanPickUp { get; set; }
        public bool IsFamilyMember { get; set; }
        public bool IsCustodian { get; set; }
        public bool IsEmergencyContact { get; set; }
        public bool IsResponsibleForBill { get; set; }
        public bool ReceivesBill { get; set; }
        public bool StudentVisibleInHome { get; set; }
        public override int DefaultOrder => 48;
    }
}
