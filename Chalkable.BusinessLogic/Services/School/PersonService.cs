using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;
using User = Chalkable.Data.Master.Model.User;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        Person Add(int localId, string email, string password, string firstName, string lastName, string gender, string salutation, DateTime? birthDate
            , int? addressId, string sisUserName, IList<SchoolPerson> assignments);
        void Add(IList<PersonInfo> persons);
        void AsssignToSchool(IList<SchoolPerson> assignments);
        IList<Person> Edit(IList<PersonInfo> personInfos);
        void Delete(int id);
        void Delete(IList<int> ids);
        void DeleteSchoolPersons(IList<SchoolPerson> schoolPersons);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        PersonDetails GetPersonDetails(int id);
        Person GetPerson(int id);
        void ActivatePerson(int id);
        Person EditEmail(int id, string email, out string error);

        IList<StudentHealthCondition> GetStudentHealthConditions(int studentId);

        StudentSummeryInfo GetStudentSummaryInfo(int studentId);
    }

    public class PersonService : SisConnectedService, IPersonService
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

        public void Add(IList<PersonInfo> persons)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
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
                        HasMedicalAlert = x.HasMedicalAlert,
                        IsAllowedInetAccess = x.IsAllowedInetAccess,
                        SpecialInstructions = x.SpecialInstructions,
                        SpEdStatus = x.SpEdStatus,
                        PhotoModifiedDate = x.PhotoModifiedDate
                    }).ToList();
                da.Insert(ps);                
                uow.Commit();
            }
        }

        public void AsssignToSchool(IList<SchoolPerson> assignments)
        {
            //TODO: need cross db transaction handling
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            var schools = ServiceLocator.ServiceLocatorMaster.SchoolService.GetSchools(Context.DistrictId.Value, 0,
                                                                                           int.MaxValue);
            //TODO: not the best performance solution for all sync but first one
            var users = ServiceLocator.ServiceLocatorMaster.UserService.GetByDistrict(Context.DistrictId.Value);
            var schoolUsers = assignments.Select(x => new SchoolUser
            {
                Id = Guid.NewGuid(),
                Role = x.RoleRef,
                SchoolRef = schools.First(y => y.LocalId == x.SchoolRef).Id,
                UserRef = users.First(y => y.LocalId == x.PersonRef).Id

            }).ToList();
            ServiceLocator.ServiceLocatorMaster.UserService.AssignUserToSchool(schoolUsers);
            using (var uow = Update())
            {
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
                if(!query.SchoolYearId.HasValue)
                    query.SchoolYearId = Context.SchoolYearId;
                var res = da.GetPersons(query);
                return res;
            }
        }


        public Person GetPerson(int id)
        {
            var res = GetPersons(new PersonQuery
                {
                    PersonId = id,
                    Count = 1, 
                    Start = 0
                }).Persons.FirstOrDefault();
            if (res == null) //in case if there is no corresponding school person
                using (var uow = Read())
                {
                    res = new PersonDataAccess(uow, Context.SchoolLocalId).GetById(id);
                }
            return res;
        }

        public PersonDetails GetPersonDetails(int id)
        {
            using (var uow = Read())
            {
                return new PersonDataAccess(uow, Context.SchoolLocalId).GetPersonDetails(id, Context.UserLocalId ?? 0, Context.Role.Id);
            }
        }
        
        public IList<Person> Edit(IList<PersonInfo> personInfos)
        {
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                var res = personInfos.Select(x => new Person
                {
                    Active = x.Active,
                    AddressRef = x.AddressRef,
                    BirthDate = x.BirthDate,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Gender = x.Gender,
                    Id = x.Id,
                    HasMedicalAlert = x.HasMedicalAlert,
                    IsAllowedInetAccess = x.IsAllowedInetAccess,
                    SpecialInstructions = x.SpecialInstructions,
                    SpEdStatus = x.SpEdStatus,
                    PhotoModifiedDate = x.PhotoModifiedDate
                }).ToList();

                da.Update(res);
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
                   || (Context.Role == CoreRoles.TEACHER_ROLE);
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
        
        public void Delete(IList<int> ids)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            if(!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                new PersonDataAccess(uow, null).Delete(ids);
                uow.Commit();
            }
            ServiceLocator.ServiceLocatorMaster.UserService.DeleteUsers(ids, Context.DistrictId.Value);
        }
        
        public void DeleteSchoolPersons(IList<SchoolPerson> schoolPersons)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolPersonDataAccess(uow).Delete(schoolPersons);
                uow.Commit();
            }
        }


        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            if (Context.Role != CoreRoles.STUDENT_ROLE)
            {
                var healsConditions = ConnectorLocator.StudentConnector.GetStudentConditions(studentId);
                return healsConditions.Select(x => new StudentHealthCondition
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsAlert = x.IsAlert,
                    MedicationType = x.MedicationType,
                    Treatment = x.Treatment
                }).ToList();   
            }
            return new List<StudentHealthCondition>();
        }

        public StudentSummeryInfo GetStudentSummaryInfo(int studentId)
        {
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var nowDashboard = ConnectorLocator.StudentConnector.GetStudentNowDashboard(syId, studentId);
            var student = GetPerson(studentId);
            var infractions = ServiceLocator.InfractionService.GetInfractions();
            var res = StudentSummeryInfo.Create(student, nowDashboard, infractions, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }
    }

    public class PersonInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Salutation { get; set; }
        public string Gender { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public int? AddressRef { get; set; }
        public string Password { get; set; }
        public string SisUserName { get; set; }

        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public DateTime? PhotoModifiedDate { get; set; }
    }
}
