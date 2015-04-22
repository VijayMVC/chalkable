using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentContact
    {
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
        [PrimaryKeyFieldAttr]
	    public int ContactRef { get; set; }
	    public int ContactRelationshipRef { get; set; }
	    public string Description { get; set; }
	    public bool ReceivesMailings { get; set; }
	    public bool CanPickUp { get; set; }
	    public bool IsFamilyMember { get; set; }
	    public bool IsCustodian { get; set; }
	    public bool IsEmergencyContact { get; set; }
	    public bool IsResponsibleForBill { get; set; }
	    public bool ReceivesBill { get; set; }
        public bool StudentVisibleInHome { get; set; }
    }
}
