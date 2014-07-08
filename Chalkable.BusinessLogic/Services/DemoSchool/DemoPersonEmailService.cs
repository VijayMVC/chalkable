using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPersonEmailService : DemoSchoolServiceBase, IPersonEmailService
    {
        public DemoPersonEmailService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        //todo : implement next methods 
        public void AddPersonsEmails(IList<PersonEmail> personEmails)
        {
            throw new NotImplementedException();
        }

        public void UpdatePersonsEmails(IList<PersonEmail> personEmails)
        {
            throw new NotImplementedException();
        }

        public void DeletePersonsEmails(IList<PersonEmail> personEmails)
        {
            throw new NotImplementedException();
        }

        public PersonEmail GetPersonEmail(int personId)
        {
            throw new NotImplementedException();
        }
    }
}
