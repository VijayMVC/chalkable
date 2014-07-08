using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPersonEmailService : DemoSchoolServiceBase, IPersonEmailService
    {
        public DemoPersonEmailService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void AddPersonsEmails(IList<PersonEmail> personEmails)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PersonEmailStorage.Add(personEmails);
        }

        public void UpdatePersonsEmails(IList<PersonEmail> personEmails)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PersonEmailStorage.Update(personEmails);
        }

        public void DeletePersonsEmails(IList<PersonEmail> personEmails)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PersonEmailStorage.Delete(personEmails);
        }

        public PersonEmail GetPersonEmail(int personId)
        {
            return Storage.PersonEmailStorage.GetAll().FirstOrDefault(x => x.PersonRef == personId);
        }
    }
}
