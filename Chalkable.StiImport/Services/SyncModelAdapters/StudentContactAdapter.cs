using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentContactAdapter : SyncModelAdapter<StudentContact>
    {
        public StudentContactAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StudentContact Selector(StudentContact x)
        {
            return new Data.School.Model.StudentContact
            {
                StudentRef = x.StudentID,
                ContactRef = x.ContactID,
                ContactRelationshipRef = x.RelationshipID,
                Description = x.Description,
                ReceivesMailings = x.ReceivesMailings,
                IsFamilyMember = x.IsFamilyMember,
                IsCustodian = x.IsCustodian,
                IsEmergencyContact = x.IsEmergencyContact,
                CanPickUp = x.CanPickUp,
                IsResponsibleForBill = x.IsResponsibleForBill,
                ReceivesBill = x.ReceivesBill,
                StudentVisibleInHome = x.StudentVisibleInHome
            };
        }

        protected override void InsertInternal(IList<StudentContact> entities)
        {
            var chlkStudentContacts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ContactService.AddStudentContact(chlkStudentContacts);
        }

        protected override void UpdateInternal(IList<StudentContact> entities)
        {
            var studentContacts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ContactService.EditStudentContact(studentContacts);
        }

        protected override void DeleteInternal(IList<StudentContact> entities)
        {
            var contacts = entities.Select(x => new Data.School.Model.StudentContact
            {
                ContactRef = x.ContactID,
                StudentRef = x.StudentID
            }).ToList();
            ServiceLocatorSchool.ContactService.DeleteStudentContact(contacts);
        }
    }
}