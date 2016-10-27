using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPersonEmailService
    {
        void AddPersonsEmails(IList<PersonEmail> personEmails);
        void UpdatePersonsEmails(IList<PersonEmail> personEmails);
        void DeletePersonsEmails(IList<PersonEmail> personEmails);
        PersonEmail GetPersonEmail(int personId);
    }

    public class PersonEmailService : SchoolServiceBase, IPersonEmailService
    {
        public PersonEmailService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddPersonsEmails(IList<PersonEmail> personEmails)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<PersonEmail>(u).Insert(personEmails));
        }

        public void UpdatePersonsEmails(IList<PersonEmail> personEmails)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<PersonEmail>(u).Update(personEmails));
        }

        public void DeletePersonsEmails(IList<PersonEmail> personEmails)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<PersonEmail>(u).Delete(personEmails));
        }

        public PersonEmail GetPersonEmail(int personId)
        {
            var res = DoRead(u => new DataAccessBase<PersonEmail>(u)
                    .GetAll(new AndQueryCondition { { PersonEmail.PERSON_REF_FIELD, personId } }));
            return res.FirstOrDefault();
        }
    }
}
