﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        void Add(IList<Person> persons);
        void Edit(IList<Person> personInfos);
        void Delete(int id);
        void Delete(IList<int> ids);
        IList<Person> GetPersons();
        PaginatedList<Person> GetPaginatedPersons(PersonQuery query); 
        PersonDetails GetPersonDetails(int id);
        Person GetPerson(int id);
        void ActivatePerson(int id);
        void EditEmail(int id, string email, out string error);
        IList<StudentHealthCondition> GetStudentHealthConditions(int studentId);
        StudentSummaryInfo GetStudentSummaryInfo(int studentId);
        IList<Person> GetAll();
        int GetSisUserId(int personId);
        IList<Person> GetTeacherStudents(int teacherId, int schoolYearId);
        IList<Person> GetClassStudents(int classId, bool? isEnrolled = null, int? markingPeriodId = null);
    }

    public class PersonService : SisConnectedService, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public IList<Person> GetAll()
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                return da.GetAll();
            }
        }

        public void Add(IList<Person> persons)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                da.Insert(persons);                
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
                query.CallerId = Context.PersonId;
                query.CallerRoleId = Context.Role.Id;
                if (!query.SchoolYearId.HasValue)
                    query.SchoolYearId = Context.SchoolYearId;
                if (query.CallerRoleId == CoreRoles.STUDENT_ROLE.Id)
                {
                    var stSchoolYear = new StudentSchoolYearDataAccess(uow).GetStudentSchoolYear(query.SchoolYearId, Context.PersonId.Value);
                    if (stSchoolYear != null)
                        query.CallerGradeLevelId = stSchoolYear.GradeLevelRef;
                }
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
            PersonDetails res;
            using (var uow = Read())
            {
                res = new PersonDataAccess(uow, Context.SchoolLocalId)
                    .GetPersonDetails(id, Context.PersonId ?? 0, Context.Role.Id);
            }
            var userId = GetSisUserId(id);
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetBySisUserId(userId, Context.DistrictId);
            res.Email = user.Login;
            return res;
        }

        public void Edit(IList<Person> persons)
        {
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                da.Update(persons);
                uow.Commit();
            }
        }


        public void EditEmail(int id, string email, out string error)
        {
            var person = GetPerson(id);
            var userId = GetSisUserId(id);
            error = null;
            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetBySisUserId(userId, Context.DistrictId);
            string oldEmail = user.Login;
            using (var uow = Update())
            {
                
                if (!(CanChangeEmail(id)))
                    throw new ChalkableSecurityException();
                if (user.Login != email)
                {
                    var newUser = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(email);
                    if (newUser != null && user.Id != newUser.Id)
                        error = "There is user with that email in Chalkable";
                    else
                    {
                        ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email);
                        ServiceLocator.ServiceLocatorMaster.EmailService.SendChangedEmailToPerson(person, oldEmail, email);
                    }
                }
                uow.Commit();
            }
        }

        private bool CanChangeEmail(int personId)
        {
            return BaseSecurity.IsAdminEditorOrCurrentPerson(personId, Context)
                   || (Context.Role == CoreRoles.TEACHER_ROLE);
        }

        public void ActivatePerson(int id)
        {
            if(!BaseSecurity.IsAdminEditorOrCurrentPerson(id, Context))
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
            if (CanGetHealthConditions())
            {
                var healthConditions = ConnectorLocator.StudentConnector.GetStudentConditions(studentId);

                if (healthConditions == null) 
                    return new List<StudentHealthCondition>();

                var result = (from studentCondition in healthConditions
                    where studentCondition != null
                    select new StudentHealthCondition
                    {
                        Id = studentCondition.Id, 
                        Name = studentCondition.Name, 
                        Description = studentCondition.Description, 
                        IsAlert = studentCondition.IsAlert, 
                        MedicationType = studentCondition.MedicationType, 
                        Treatment = studentCondition.Treatment
                    }).ToList();
                return result;
            }
            return new List<StudentHealthCondition>();
        }

        private bool CanGetHealthConditions()
        {
            return Context.Role != CoreRoles.STUDENT_ROLE
                   && (ClaimInfo.HasPermission(Context.Claims, new List<string> {ClaimInfo.VIEW_HEALTH_CONDITION})
                       || ClaimInfo.HasPermission(Context.Claims, new List<string> {ClaimInfo.VIEW_MEDICAL}));
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId)
        {
            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var nowDashboard = ConnectorLocator.StudentConnector.GetStudentNowDashboard(syId, studentId);
            var student = GetPerson(studentId);
            var infractions = ServiceLocator.InfractionService.GetInfractions();
            var activitiesIds = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();
            IList<AnnouncementComplex> anns;
            using (var uow = Read())
            {
                var da = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId);
                anns = da.GetByActivitiesIds(activitiesIds);
            }
            var res = StudentSummaryInfo.Create(student, nowDashboard, infractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }


        public int GetSisUserId(int personId)
        {
            using (var uow = Read())
            {
                var student = new StudentDataAccess(uow).GetByIdOrNull(personId);
                if (student != null) return student.UserId;
                var staff = new StaffDataAccess(uow).GetByIdOrNull(personId);
                if (staff != null && staff.UserId.HasValue) return staff.UserId.Value;
                var person = new PersonDataAccess(uow, Context.SchoolLocalId).GetByIdOrNull(personId);
                if (person != null && person.UserId.HasValue) return person.UserId.Value;

                throw new ChalkableException("Current person doesn't have user data");
            }
        }

        public IList<Person> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            using (var uow = Read())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                return da.GetTeacherStudents(teacherId, schoolYearId);
            }
        }

        public IList<Person> GetClassStudents(int classId, bool? isEnrolled = null, int? markingPeriodId = null)
        {
            using (var uow = Read())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                return da.GetStudents(classId, isEnrolled, markingPeriodId);
            }
        }
    }
}
