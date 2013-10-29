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
        Person Add(int localId, int schoolId, string email, string password, string firstName, string lastName, string role, string gender, string salutation, DateTime? birthDate, int? gradeLevelId, int? sisId = null);
        Person Edit(int localId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate);

        Person EditStudent(int studentId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate
            ,bool iep, DateTime enrollmentDate, string previousSchool, string previousSchoolPhone, string previousSchoolNote, int? gradeLevelId);

        void Delete(int id);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        //PaginatedList<Person> GetPersons(int? roleId, Guid? classId, IList<Guid> gradeLevelId, string filter, SortTypeEnum sortType = SortTypeEnum.ByLastName, int start = 0, int count = int.MaxValue); 
        Person GetPerson(int id);
        PersonDetails GetPersonDetails(int id);
        void ActivatePerson(int id);
    }

    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs tests
        public Person Add(int localId, int schoolId, string email, string password, string firstName, string lastName, string role, string gender, string salutation, DateTime? birthDate, int? gradeLevelId, int? sisId = null)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            //TODO: need cross db transaction handling
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                var roleId = CoreRoles.GetByName(role).Id;
                var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.SchoolId.Value, role, localId);
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
                    };
                da.Insert(person);
                new SchoolPersonDataAccess(uow).Insert(new SchoolPerson
                    {
                        PersonRef = localId,
                        RoleRef = roleId,
                        SchoolRef = schoolId
                    });
                if (role == CoreRoles.STUDENT_ROLE.Name)
                {
                    //if (gradeLevelId.HasValue)
                    //{
                    //    var ssyDa = new StudentSchoolYearDataAccess(uow);
                    //    ssyDa.Insert(new StudentSchoolYear
                    //        {
                    //            GradeLevelRef = gradeLevelId.Value,
                    //            StudentRef = person.Id,

                    //        });                    
                    //}

                    //else
                    //    throw new ChalkableException("Grade level is required for adding student");
                }
                new SchoolPersonDataAccess(uow).Insert(new SchoolPerson
                    {
                        SchoolRef = schoolId,
                        PersonRef = person.Id,
                        RoleRef = roleId
                    });
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
                var da = new PersonDataAccess(uow);
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
                var da = new PersonDataAccess(uow);
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

        //public PaginatedList<Person> GetPersons(int? roleId, Guid? classId, IList<Guid> gradeLevelId, string filter, SortTypeEnum sortType = SortTypeEnum.ByLastName, int start = 0, int count = int.MaxValue)
        //{
        //   return GetPaginatedPersons(new PersonQuery
        //        {
        //            RoleId = roleId,
        //            ClassId = classId,
        //            GradeLevelIds = gradeLevelId,
        //            Filter = filter,
        //            SortType = sortType,
        //            Start = start,
        //            Count = count
        //        });
        //}

        public PersonDetails GetPersonDetails(int id)
        {
            using (var uow = Read())
            {
                return new PersonDataAccess(uow).GetPersonDetails(id, Context.UserLocalId ?? 0, Context.Role.Id);
            }
        }

        public Person Edit(int localId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate)
        {
            using (var uow = Update())
            {
                var res = Edit(new PersonDataAccess(uow), localId, email, firstName, lastName, gender, salutation, birthDate);
                uow.Commit();
                return res;
            }
        }

        public Person EditStudent(int studentId, string email, string firstName, string lastName, string gender, string salutation, 
            DateTime? birthDate, bool iep, DateTime enrollmentDate, string previousSchool, string previousSchoolPhone, 
            string previousSchoolNote, Guid? gradeLevelId)
        {
            if(!(BaseSecurity.IsAdminOrTeacher(Context) || Context.UserLocalId == studentId))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var student = Edit(new PersonDataAccess(uow), studentId, email, firstName, lastName, gender, salutation, birthDate);
                //if (gradeLevelId.HasValue)
                //    student.StudentInfo.GradeLevelRef = gradeLevelId.Value;

                //new StudentInfoDataAccess(uow).Update(student.StudentInfo);
                uow.Commit();
                return student;
            }
        }

        private Person Edit(PersonDataAccess dataAccess, int localId, string email, string firstName
                    , string lastName, string gender, string salutation, DateTime? birthDate)
        {
            var res = GetPerson(localId);
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(res.Email);
            ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
            res.FirstName = firstName;
            res.LastName = lastName;
            res.Gender = gender;
            res.Salutation = salutation;
            res.BirthDate = birthDate;
            dataAccess.Update(res);
            return res;
        }


        public void ActivatePerson(int id)
        {
            if(BaseSecurity.IsAdminEditorOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
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
