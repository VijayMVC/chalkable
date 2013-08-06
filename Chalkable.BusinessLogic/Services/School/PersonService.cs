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
        Person Add(string email, string password, string firstName, string lastName, string role, string gender, string salutation, DateTime? birthDate, Guid? gradeLevelId, int? sisId = null);
        Person Edit(Guid personId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate);

        Person EditStudent(Guid studentId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate
            ,bool iep, DateTime enrollmentDate, string previousSchool, string previousSchoolPhone, string previousSchoolNote, Guid? gradeLevelId);

        void Delete(string id);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        //PaginatedList<Person> GetPersons(int? roleId, Guid? classId, IList<Guid> gradeLevelId, string filter, SortTypeEnum sortType = SortTypeEnum.ByLastName, int start = 0, int count = int.MaxValue); 
        Person GetPerson(Guid id);
        PersonDetails GetPersonDetails(Guid id);
        void ActivatePerson(Guid id);
    }

    public class PersonService : SchoolServiceBase, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO: needs tests
        public Person Add(string email, string password, string firstName, string lastName, string role, string gender, string salutation, DateTime? birthDate, Guid? gradeLevelId, int? sisId = null)
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
                var user = ServiceLocator.ServiceLocatorMaster.UserService.CreateSchoolUser(email, password, Context.SchoolId.Value, role);
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
                        RoleRef = roleId,
                        SisId = sisId
                    };
                da.Insert(person);
                if (role == CoreRoles.STUDENT_ROLE.Name)
                {
                    if (gradeLevelId.HasValue)
                        da.AddStudent(user.Id, gradeLevelId.Value);
                    else
                        throw new ChalkableException("Grade level is required for adding student");
                }
                uow.Commit();
                return person;
            }
        }
        
        public void Delete(string id)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                da.Delete(Guid.Parse(id));
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
                query.CallerId = Context.UserId;
                query.CallerRoleId = Context.Role.Id;
                var res = da.GetPersons(query);
                return res;
            }
        } 


        public Person GetPerson(Guid id)
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

        public PersonDetails GetPersonDetails(Guid id)
        {
            using (var uow = Read())
            {
                return new PersonDataAccess(uow).GetPersonDetails(id, Context.UserId, Context.Role.Id);
            }
        }

        public Person Edit(Guid personId, string email, string firstName, string lastName, string gender, string salutation, DateTime? birthDate)
        {
            using (var uow = Update())
            {
                var res = Edit(new PersonDataAccess(uow), personId, email, firstName, lastName, gender, salutation, birthDate);
                uow.Commit();
                return res;
            }
        }

        public Person EditStudent(Guid studentId, string email, string firstName, string lastName, string gender, string salutation, 
            DateTime? birthDate, bool iep, DateTime enrollmentDate, string previousSchool, string previousSchoolPhone, 
            string previousSchoolNote, Guid? gradeLevelId)
        {
            if(!(BaseSecurity.IsAdminOrTeacher(Context) || Context.UserId == studentId))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var student = Edit(new PersonDataAccess(uow), studentId, email, firstName, lastName, gender, salutation, birthDate);
                student.StudentInfo.IEP = iep;
                student.StudentInfo.EnrollmentDate = enrollmentDate;
                student.StudentInfo.PreviousSchool = previousSchool;
                student.StudentInfo.PreviousSchoolNote = previousSchoolNote;
                student.StudentInfo.PreviousSchoolPhone = previousSchoolPhone;
                if (gradeLevelId.HasValue)
                    student.StudentInfo.GradeLevelRef = gradeLevelId.Value;

                new StudentInfoDataAccess(uow).Update(student.StudentInfo);
                uow.Commit();
                return student;
            }
        }

        private Person Edit(PersonDataAccess dataAccess, Guid personId, string email, string firstName
                    , string lastName, string gender, string salutation, DateTime? birthDate)
        {
            ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(personId, email);
            var res = GetPerson(personId);
            res.FirstName = firstName;
            res.LastName = lastName;
            res.Gender = gender;
            res.Salutation = salutation;
            res.BirthDate = birthDate;
            dataAccess.Update(res);
            return res;
        }


        public void ActivatePerson(Guid id)
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
