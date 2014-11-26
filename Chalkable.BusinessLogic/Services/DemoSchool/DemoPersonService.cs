using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPersonService : DemoSchoolServiceBase, IPersonService
    {

        public DemoPersonService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }
        
        public PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count)
        {
            return Storage.PersonStorage.SearchPersons(filter, orderByFirstName, start, count);
        }

        public void Add(IList<Person> persons)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<Person> persons, IList<SchoolPerson> assignments)
        {
            throw new NotImplementedException();
            /*if (!BaseSecurity.IsAdminEditor(Context))
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
            ServiceLocator.ServiceLocatorMaster.UserService.AddSchoolUsers(schoolUsers);

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
            Storage.SchoolPersonStorage.Add(assignments);*/
        }

        public void Edit(IList<Person> personInfos)
        {
            //var res = new List<Person>();
            //foreach (var personInfo in personInfos)
            //{
            //    res.Add(Edit(personInfo.Id, personInfo.Email, personInfo.FirstName,
            //                 personInfo.LastName, personInfo.Gender, personInfo.Salutation,
            //                 personInfo.BirthDate, personInfo.AddressRef));
            //}
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

        public PaginatedList<Person> GetPaginatedPersons(PersonQuery query)
        {
            var res = GetPersons(query);
            return new PaginatedList<Person>(res.Persons, query.Start/query.Count, query.Count, res.SourceCount);
        }

        private PersonQueryResult GetPersons(PersonQuery query)
        {
            query.CallerId = Context.PersonId;
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
            return Storage.PersonStorage.GetPersonDetails(id, Context.PersonId ?? 0, Context.Role.Id);
        }

        public Person EditEmail(int id, string email, out string error)
        {
            throw new NotImplementedException();
            //var res = GetPerson(id);
            //error = null;
            //if (!(CanChangeEmail(res)))
            //    throw new ChalkableSecurityException();
            //if (res.Email != email)
            //{
            //    if (Storage.PersonStorage.Exists(email, res.Id))
            //        error = "There is user with that email in Chalkable";
            //    else
            //    {
            //        res.Email = email;
            //        Storage.PersonStorage.Update(res);
            //    }
            //}
            //return res;
        }

        private bool CanChangeEmail(Person person)
        {
            return BaseSecurity.IsAdminEditorOrCurrentPerson(person.Id, Context)
                   || (Context.Role == CoreRoles.TEACHER_ROLE && person.RoleRef == CoreRoles.STUDENT_ROLE.Id);
        }

        public Person Edit(int localId, string email, string firstName,
            string lastName, string gender, string salutation, DateTime? birthDate, int? addressId)
        {
            //var res = GetPerson(localId);
            //var user = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(res.Email);
            //ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
            //res.FirstName = firstName;
            //res.LastName = lastName;
            //res.Gender = gender;
            //res.Salutation = salutation;
            //res.BirthDate = birthDate;
            //res.AddressRef = addressId;
            //Storage.PersonStorage.Update(res);
            //return res;
            throw new NotImplementedException();
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

        public IList<Person> GetAll()
        {
            throw new NotImplementedException();
        }


        void IPersonService.EditEmail(int id, string email, out string error)
        {
            throw new NotImplementedException();
        }

        public int GetSisUserId(int personId)
        {
            throw new NotImplementedException();
        }
    }
}
