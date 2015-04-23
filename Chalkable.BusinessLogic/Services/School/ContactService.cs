﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IContactService
    {
        void AddStudentContact(IList<StudentContact> studentContacts);
        void EditStudentContact(IList<StudentContact> studentContacts);
        void DeleteStudentContact(IList<StudentContact> studentContacts);

        void AddContactRelationship(IList<ContactRelationship> contactRelationships);
        void EditContactRelationship(IList<ContactRelationship> contactRelationships);
        void DeleteContactRelationship(IList<ContactRelationship> contactRelationships);
    }

    public class ContactService : SchoolServiceBase, IContactService
    {
        public ContactService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStudentContact(IList<StudentContact> studentContacts)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentContact>(u).Insert(studentContacts));
        }

        public void EditStudentContact(IList<StudentContact> studentContacts)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentContact>(u).Update(studentContacts));
        }

        public void DeleteStudentContact(IList<StudentContact> studentContacts)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentContact>(u).Delete(studentContacts));
        }

        public void AddContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ContactRelationship>(u).Insert(contactRelationships));
        }

        public void EditContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ContactRelationship>(u).Update(contactRelationships));
        }

        public void DeleteContactRelationship(IList<ContactRelationship> contactRelationships)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<ContactRelationship>(u).Delete(contactRelationships));
        }
    }
}
