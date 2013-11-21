using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        Person Add(int localId, string email, string password, string firstName, string lastName, string gender, string salutation, DateTime? birthDate
            , int? addressId, string sisUserName, IList<SchoolPerson> assignments);
        void Add(IList<PersonInfo> persons, IList<SchoolPerson> assignments);
        Person Edit(int localId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate, int? addressId);
        void Delete(int id);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        Person GetPerson(int id);
        PersonDetails GetPersonDetails(int id);
        void ActivatePerson(int id);
        Person EditEmail(int id, string email, out string error);

    }
    
    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs tests
        public Person Add(int localId, string email, string password, string firstName, string lastName, string gender, string salutation, DateTime? birthDate
            , int? addressId, string sisUserName, IList<SchoolPerson> assignments)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            var schools = ServiceLocator.ServiceLocatorMaster.SchoolService.GetAll();
            //TODO: need cross db transaction handling
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                
                var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.DistrictId.Value, localId, sisUserName);
                ServiceLocator.ServiceLocatorMaster.UserService.AssignUserToSchool(assignments.Select(x => new SchoolUser
                {
                    Id = Guid.NewGuid(),
                    Role = x.RoleRef,
                    SchoolRef = schools.First(y=>y.LocalId == x.SchoolRef).Id,
                    UserRef = user.Id
                }).ToList());

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
                schoolDataAccess.Insert(assignments);
                uow.Commit();
                return person;
            }
        }

        public void Add(IList<PersonInfo> persons, IList<SchoolPerson> assignments)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            //TODO: need cross db transaction handling
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);

                var users = persons.Select(x => new User
                    {
                        LocalId = x.Id,
                        Login = x.Email,
                        DistrictRef = Context.DistrictId.Value,
                        Password = x.Password,
                        SisUserName = x.SisUserName,
                        Id = Guid.NewGuid()
                    }).ToList();

                ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUsers(users);

                var schools = ServiceLocator.ServiceLocatorMaster.SchoolService.GetSchools(Context.DistrictId.Value, 0,
                                                                                           int.MaxValue);

                var schoolUsers = assignments.Select(x => new SchoolUser
                    {
                        Id = Guid.NewGuid(),
                        Role = x.RoleRef,
                        SchoolRef = schools.First(y=>y.LocalId == x.SchoolRef).Id,
                        UserRef = users.First(y => y.LocalId == x.PersonRef).Id

                    }).ToList();
                ServiceLocator.ServiceLocatorMaster.UserService.AssignUserToSchool(schoolUsers);

                var ps = persons.Select(x => new Person
                    {
                        Active = x.Active,
                        AddressRef = x.AddressRef,
                        BirthDate = x.BirthDate,
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Gender = x.Gender,
                        Id = x.Id,
                    }).ToList();
                da.Insert(ps);
                var schoolDataAccess = new SchoolPersonDataAccess(uow);
                schoolDataAccess.Insert(assignments);
                uow.Commit();
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

        public Person EditEmail(int id, string email, out string error)
        {
            using (var uow = Update())
            {
                var res = EditEmail(new PersonDataAccess(uow, Context.SchoolLocalId), id, email, out  error);
                uow.Commit();
                return res;
            }
        }

        private Person EditEmail(PersonDataAccess dataAccess, int id, string email, out string error)
        {
            var res = GetPerson(id);
            error = null;
            if (!(CanChangeEmail(res)))
                throw new ChalkableSecurityException();
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(res.Email);
            if (res.Email != email)
            {
                if (dataAccess.Exists(email, res.Id))
                    error = "There is user with that email in Chalkable";
                else
                {
                    ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
                    res.Email = email;
                    dataAccess.Update(res);
                    ServiceLocator.ServiceLocatorMaster.EmailService.SendChangedEmailToPerson(res, email);         
                }
            }
            return res;
        }

        private bool CanChangeEmail(Person person)
        {
            return BaseSecurity.IsAdminEditorOrCurrentPerson(person.Id, Context)
                   || (Context.Role == CoreRoles.TEACHER_ROLE && person.RoleRef == CoreRoles.STUDENT_ROLE.Id);
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

    public class PersonInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public int? AddressRef { get; set; }
        public string Password { get; set; }
        public string SisUserName { get; set; }
    }
}
