using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        void Add(string email, string firstName, string lastName, string localId, int? schoolId, string role, string gender, string salutation, DateTime? birthDate);
        void Delete(string id);
    }

    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(string email, string firstName, string lastName, string localId, int? schoolId, string role, string gender, string salutation, DateTime? birthDate)
        {
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                var person = new Person
                    {
                        Active = false,
                        Email = email,
                        BirthDate = birthDate,
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = gender,
                        Salutation = salutation,
                        RoleRef = CoreRoles.GetByName(role).Id
                    };
                da.Create(person);
                uow.Commit();
            }
        }
        
        public void Delete(string id)
        {
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                da.Delete(da.GetById(Guid.Parse(id)));
                uow.Commit();
            }
        }
    }
}
