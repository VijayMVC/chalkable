using System;
using System.Collections.Generic;
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
    }
}
