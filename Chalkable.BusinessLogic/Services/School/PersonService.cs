using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        Person Add(int localId, string email, string password, string firstName, string lastName, string gender, string salutation, DateTime? birthDate, int? addressId, IList<SchoolAssignmentInfo> assignments);
        Person Edit(int localId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate, int? addressId);
        void Delete(int id);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        Person GetPerson(int id);
        PersonDetails GetPersonDetails(int id);
        void ActivatePerson(int id);
    }

    public class SchoolAssignmentInfo
    {
        public int Role { get; set; }
        public int SchoolId { get; set; }
    }

    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs tests
        public Person Add(int localId, string email, string password, string firstName, string lastName, string gender, string salutation, DateTime? birthDate, int? addressId, IList<SchoolAssignmentInfo> assignments)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            //TODO: need cross db transaction handling
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                
                var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.DistrictId.Value, localId);
                foreach (var assignment in assignments)
                {
                    ServiceLocator.ServiceLocatorMaster.UserService.AssignUserToSchool(user.Id, assignment.SchoolId, assignment.Role);
                }

                var person = new Person
                    {
                        Id = localId,
                        Active = false,
                        Email = email,
                        BirthDate = birthDate,
                        FirstName = firstName,
                        LastName = lastName,
                        Gender = gender,
                        Salutation = salutation,
                        AddressRef = addressId
                    };
                da.Insert(person);
                var schoolDataAccess = new SchoolPersonDataAccess(uow);
                foreach (var assignment in assignments)
                {
                    schoolDataAccess.Insert(new SchoolPerson
                    {
                        SchoolRef = assignment.SchoolId,
                        PersonRef = person.Id,
                        RoleRef = assignment.Role
                    });
                }

                
                uow.Commit();
                return person;
            }
        }
        
        public void Delete(int id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                da.Delete(id);
                uow.Commit();
            }
        }

        public IList<Person> GetPersons()
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();

            return GetPersons(new PersonQuery 
                {
                    Count = int.MaxValue,
                    Start = 0
                }).Persons;
        }

        public PaginatedList<Person> GetPaginatedPersons(PersonQuery query)
        {
            var res = GetPersons(query);
            return new PaginatedList<Person>(res.Persons, query.Start / query.Count, query.Count, res.SourceCount);
        }

        private PersonQueryResult GetPersons(PersonQuery query)
        {
            using (var uow = Read())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                query.CallerId = Context.UserLocalId;
                query.CallerRoleId = Context.Role.Id;
                var res = da.GetPersons(query);
                return res;
            }
        }


        public Person GetPerson(int id)
        {
            return GetPersons(new PersonQuery
                {
                    PersonId = id,
                    Count = 1, 
                    Start = 0
                }).Persons.First();
        }

        public PersonDetails GetPersonDetails(int id)
        {
            using (var uow = Read())
            {
                return new PersonDataAccess(uow, Context.SchoolLocalId).GetPersonDetails(id, Context.UserLocalId ?? 0, Context.Role.Id);
            }
        }

        public Person Edit(int localId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate, int? addressId)
        {
            using (var uow = Update())
            {
                var res = Edit(new PersonDataAccess(uow, Context.SchoolLocalId), localId, email, firstName, lastName, gender, salutation, birthDate, addressId);
                uow.Commit();
                return res;
            }
        }

        private Person Edit(PersonDataAccess dataAccess, int localId, string email, string firstName
                    , string lastName, string gender, string salutation, DateTime? birthDate, int? addressId)
        {
            var res = GetPerson(localId);
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(res.Email);
            ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
            res.FirstName = firstName;
            res.LastName = lastName;
            res.Gender = gender;
            res.Salutation = salutation;
            res.BirthDate = birthDate;
            res.AddressRef = addressId;
            dataAccess.Update(res);
            return res;
        }


        public void ActivatePerson(int id)
        {
            if(BaseSecurity.IsAdminEditorOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                var person = GetPerson(id);
                //TODO: change school status 
                person.Active = true;
                person.FirstLoginDate = Context.NowSchoolTime;
                da.Update(person);
                uow.Commit();
            }
        }
    }
}
