using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
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
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new PersonEmailDataAccess(uow).Insert(personEmails);
                uow.Commit();
            }
        }

        public void UpdatePersonsEmails(IList<PersonEmail> personEmails)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new PersonEmailDataAccess(uow).Update(personEmails);
                uow.Commit();
            }
        }

        public void DeletePersonsEmails(IList<PersonEmail> personEmails)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new PersonEmailDataAccess(uow).Delete(personEmails);
                uow.Commit();
            }
        }

        public PersonEmail GetPersonEmail(int personId)
        {
            using (var uow = Read())
            {
                return new PersonEmailDataAccess(uow)
                    .GetAll(new AndQueryCondition {{PersonEmail.PERSON_REF_FIELD, personId}}).FirstOrDefault();
            }
        }
    }
}
