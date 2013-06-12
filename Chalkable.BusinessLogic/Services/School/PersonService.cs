using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        void Add(string email, string password, string firstName, string lastName, string localId, int? schoolId, string role, string gender, string salutation, DateTime? birthDate);
        void Delete(string id);
    }

    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs tests
        public void Add(string email, string password, string firstName, string lastName, string localId, int? schoolId, string role, string gender, string salutation, DateTime? birthDate)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                var roleId = CoreRoles.GetByName(role).Id;
                var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.SchoolId.ToString(), role);
                
                var person = new Person
                    {
                        Id = user.Id,
                        Active = false,
                        Email = email,
                        BirthDate = birthDate,
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = gender,
                        Salutation = salutation,
                        RoleRef = roleId
                    };
                da.Create(person);
                uow.Commit();
            }
        }
        
        public void Delete(string id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                da.Delete(da.GetById(Guid.Parse(id)));
                uow.Commit();
            }
        }
    }
}
