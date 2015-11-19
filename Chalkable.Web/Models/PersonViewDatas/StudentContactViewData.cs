using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentContactViewData 
    {
        public PersonInfoViewData PersonInfo { get; set; }
        public string RelationshipName { get; set; }
        public bool IsFamilyMember { get; set; }
        public bool IsEmergencyContact { get; set; }
        public bool IsResponsibleForBill { get; set; }
        public bool IsAllowedToPickup { get; set; }
        public bool IsCustodian { get; set; }
        public bool ReceivesMailings { get; set; }
        public bool ReceivesBill { get; set; }

        public static IList<StudentContactViewData> Create(IList<StudentContactDetails> studentContacts)
        {
            return studentContacts.Select(x => new StudentContactViewData
                {
                    PersonInfo = PersonInfoViewData.Create(x.Person),
                    RelationshipName = x.ContactRelationship.Name,
                    IsFamilyMember = x.IsFamilyMember,
                    IsAllowedToPickup = x.CanPickUp,
                    IsEmergencyContact = x.IsEmergencyContact,
                    IsResponsibleForBill = x.IsResponsibleForBill,
                    IsCustodian = x.IsCustodian,
                    ReceivesBill = x.ReceivesBill,
                    ReceivesMailings = x.ReceivesMailings
                }).ToList();
        } 
    }
}