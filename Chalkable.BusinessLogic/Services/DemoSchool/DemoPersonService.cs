using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPersonService : DemoSchoolServiceBase, IPersonService
    {

        public DemoPersonService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public Person Add(int localId, string email, string password, string firstName, string lastName, string gender,
            string salutation, DateTime? birthDate
            , int? addressId, string sisUserName, IList<SchoolPerson> assignments)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            var schools = ServiceLocator.ServiceLocatorMaster.SchoolService.GetAll();


            var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.DistrictId.Value, localId, sisUserName);
            ServiceLocator.ServiceLocatorMaster.UserService.AssignUserToSchool(assignments.Select(x => new SchoolUser
            {
                Id = Guid.NewGuid(),
                Role = x.RoleRef,
                SchoolRef = schools.First(y => y.LocalId == x.SchoolRef).Id,
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
            Storage.PersonStorage.Add(person);
            Storage.SchoolPersonStorage.Add(assignments);
            return person;
        }

        public void Add(IList<PersonInfo> persons)
        {
            throw new NotImplementedException();
        }

        public void AsssignToSchool(IList<SchoolPerson> assignments)
        {
            throw new NotImplementedException();
        }


        public void Add(IList<PersonInfo> persons, IList<SchoolPerson> assignments)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
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
                SchoolRef = schools.First(y => y.LocalId == x.SchoolRef).Id,
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
            Storage.PersonStorage.Add(ps);
            Storage.SchoolPersonStorage.Add(assignments);
        }

        public IList<Person> Edit(IList<PersonInfo> personInfos)
        {
            var res = new List<Person>();
            foreach (var personInfo in personInfos)
            {
                res.Add(Edit(personInfo.Id, personInfo.Email, personInfo.FirstName,
                             personInfo.LastName, personInfo.Gender, personInfo.Salutation,
                             personInfo.BirthDate, personInfo.AddressRef));
            }
            return Storage.PersonStorage.Update(res);

        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            Storage.PersonStorage.Delete(id);
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            Storage.PersonStorage.Delete(ids);
        }

        public void DeleteSchoolPersons(IList<SchoolPerson> schoolPersons)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.SchoolPersonStorage.Delete(schoolPersons);
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
            return new PaginatedList<Person>(res.Persons, query.Start/query.Count, query.Count, res.SourceCount);
        }

        private PersonQueryResult GetPersons(PersonQuery query)
        {
            query.CallerId = Context.UserLocalId;
            query.CallerRoleId = Context.Role.Id;
            return Storage.PersonStorage.GetPersons(query);
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
            return Storage.PersonStorage.GetPersonDetails(id, Context.UserLocalId ?? 0, Context.Role.Id);
        }

        public Person EditEmail(int id, string email, out string error)
        {
            var res = GetPerson(id);
            error = null;
            if (!(CanChangeEmail(res)))
                throw new ChalkableSecurityException();
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(res.Email);
            if (res.Email != email)
            {
                if (Storage.PersonStorage.Exists(email, res.Id))
                    error = "There is user with that email in Chalkable";
                else
                {
                    ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
                    res.Email = email;
                    Storage.PersonStorage.Update(res);
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

        public Person Edit(int localId, string email, string firstName,
            string lastName, string gender, string salutation, DateTime? birthDate, int? addressId)
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
            Storage.PersonStorage.Update(res);
            return res;
        }


        public void ActivatePerson(int id)
        {
            if (BaseSecurity.IsAdminEditorOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            var person = GetPerson(id);
            person.Active = true;
            person.FirstLoginDate = Context.NowSchoolTime;
            Storage.PersonStorage.Update(person);
        }


        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            return Storage.StudentHealthConditionStorage.GetByStudentId(studentId);
        }
    }
}
