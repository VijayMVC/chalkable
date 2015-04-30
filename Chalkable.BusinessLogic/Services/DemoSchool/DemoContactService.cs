using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoContactService : DemoSchoolServiceBase, IContactService
    {
        public DemoContactService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }
        public void AddStudentContact(IList<StudentContact> studentContacts)
        {
            throw new NotImplementedException();
        }

        public void EditStudentContact(IList<StudentContact> studentContacts)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudentContact(IList<StudentContact> studentContacts)
        {
            throw new NotImplementedException();
        }

        public void AddContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            throw new NotImplementedException();
        }

        public void EditContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            throw new NotImplementedException();
        }

        public void DeleteContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            throw new NotImplementedException();
        }

        public IList<StudentContactDetails> GetStudentContactDetails(int studentId)
        {
            var stContacts = Storage.StudentContactStorage.GetAll().Where(x => x.StudentRef == studentId).ToList();
            var res = new List<StudentContactDetails>();
            foreach (var studentContact in stContacts)
            {
                var person = ServiceLocator.PersonService.GetPersonDetails(studentContact.ContactRef);
                var contactRelationship = Storage.ContactRelationshipStorage.GetById(studentContact.ContactRelationshipRef);
                res.Add(new StudentContactDetails
                    {
                        StudentRef = studentContact.StudentRef,
                        ContactRef = studentContact.ContactRef,
                        CanPickUp = studentContact.CanPickUp,
                        ContactRelationshipRef = studentContact.ContactRelationshipRef,
                        Description = studentContact.Description,
                        IsCustodian = studentContact.IsCustodian,
                        IsEmergencyContact = studentContact.IsEmergencyContact,
                        IsFamilyMember = studentContact.IsFamilyMember,
                        IsResponsibleForBill = studentContact.IsResponsibleForBill,
                        ReceivesBill = studentContact.ReceivesBill,
                        ReceivesMailings = studentContact.ReceivesMailings,
                        StudentVisibleInHome = studentContact.StudentVisibleInHome,
                        ContactRelationship = contactRelationship,
                        Person = person
                    });
            }
            return res;
        }
    }
}
