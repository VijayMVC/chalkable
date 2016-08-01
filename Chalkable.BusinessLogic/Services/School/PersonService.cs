using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPersonService
    {
        void Add(IList<Person> persons);
        void UpdateForImport(IList<Person> persons);
        void Delete(IList<Person> persons);
        PersonDetails GetPersonDetails(int id, int? schoolId = null);
        Person GetPerson(int id);
        void ActivatePerson(int id);
        void ProcessPersonFirstLogin(int id);
        void EditEmailForCurrentUser(int personId, string email, out string error);
        IList<Person> GetAll();
        PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count);

        CoreRole GetPersonRole(int personId);
    }

    public class PersonService : SisConnectedService, IPersonService
    {
        public PersonService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public IList<Person> GetAll()
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(u => new PersonDataAccess(u).GetAll());
        }

        public void Add(IList<Person> persons)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();

            DoUpdate(u => new PersonDataAccess(u).Insert(persons));
        }
        
        public Person GetPerson(int id)
        {
            return DoRead(u => new PersonDataAccess(u).GetById(id));
        }

        public PersonDetails GetPersonDetails(int id, int? schoolId = null)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var res = DoRead(uow => new PersonDataAccess(uow).GetPersonDetails(id, schoolId ?? Context.SchoolLocalId.Value));

            if (BaseSecurity.IsDistrictAdmin(Context) && Context.PersonId == res.Id)
                res.RoleRef = Context.Role.Id;
            return res;
        }

        public void EditEmailForCurrentUser(int personId, string email, out string error)
        {
            Trace.Assert(Context.PersonId.HasValue);
            
            var person = GetPersonDetails(personId);
            if(!person.UserId.HasValue)
                throw new ChalkableSecurityException();

            var user = ServiceLocator.ServiceLocatorMaster.UserService.GetBySisUserId(person.UserId.Value, Context.DistrictId);
            string oldEmail = user.Login;

            using (var uow = Update())
            {
                ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(user.Id, email, out error); //security check here
                if(string.IsNullOrEmpty(error))
                    return;

                var stiPersonEmail = new StiPersonEmail
                        {
                            Description = "",
                            EmailAddress = email,
                            IsListed = true,
                            IsPrimary = true,
                            PersonId = Context.PersonId.Value
                        };
                ConnectorLocator.UsersConnector.UpdatePrimaryPersonEmail(stiPersonEmail.PersonId, stiPersonEmail);
                ServiceLocator.ServiceLocatorMaster.EmailService.SendChangedEmailToPerson(person, oldEmail, email);
                
                uow.Commit();
            }
        }


        public void ActivatePerson(int id)
        {
            if(!BaseSecurity.IsDistrictAdminOrCurrentPerson(id, Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new PersonDataAccess(uow);
                var person = GetPerson(id);
                //TODO: change school status 
                person.Active = true;
                //person.FirstLoginDate = Context.NowSchoolTime;
                da.Update(person);
                uow.Commit();
            }
        }

        public void Delete(IList<Person> persons)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            DoUpdate(u=> new PersonDataAccess(u).Delete(persons.Select(x => x.Id).ToList()));
        }
        
        public void DeleteSchoolPersons(IList<SchoolPerson> schoolPersons)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=> new SchoolPersonDataAccess(u).Delete(schoolPersons));
        }

        public PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            return DoRead(u => new PersonDataAccess(u).SearchPersons(Context.SchoolLocalId.Value, filter, orderByFirstName, start, count));
        }

        public void ProcessPersonFirstLogin(int id)
        {
            if (Context.PersonId != id)
                throw new ChalkableSecurityException();
            DoUpdate(uow =>
            {
                var da = new PersonDataAccess(uow);
                var p = da.GetById(id);
                if (p.FirstLoginDate.HasValue) return;
                p.FirstLoginDate = Context.NowSchoolTime;
                da.Update(p);
                ServiceLocator.ClassAnnouncementService.SetAnnouncementsAsComplete(Context.NowSchoolTime, true);
                LessonPlanService.SetAnnouncementsAsComplete(uow, ServiceLocator, Context.NowSchoolTime, true);
                AdminAnnouncementService.SetAnnouncementsAsComplete(uow, ServiceLocator, Context.NowSchoolTime, true);
            });
        }

        public void UpdateForImport(IList<Person> persons)
        {
            DoUpdate(uow => new PersonDataAccess(uow).UpdateForImport(persons));
        }

        public CoreRole GetPersonRole(int personId)
        {
            var isTeacher = DoRead(u => new StaffDataAccess(u).GetByIdOrNull(personId));
            var isStudent = DoRead(u => new StudentDataAccess(u).GetByIdOrNull(personId));

            if (isTeacher != null)
                return CoreRoles.TEACHER_ROLE;

            if (isStudent != null)
                return CoreRoles.STUDENT_ROLE;

            return null;
        }
    }
}
