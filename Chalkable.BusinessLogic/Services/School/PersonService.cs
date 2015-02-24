﻿using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
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
        void Edit(IList<Person> personInfos);
        void UpdateForImport(IList<Person> persons);
        void Delete(int id);
        void Delete(IList<int> ids);
        PersonDetails GetPersonDetails(int id);
        Person GetPerson(int id);
        void ActivatePerson(int id);
        void ProcessPersonFirstLogin(int id);
        void EditEmailForCurrentUser(string email, out string error);
        IList<Person> GetAll();
        int GetSisUserId(int personId);
        PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count);
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

        public Person GetPerson(int id)
        {
            using (var uow = Read())
            {
                return new PersonDataAccess(uow, null).GetById(id);
            }
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


        public void EditEmailForCurrentUser(string email, out string error)
        {
            Trace.Assert(Context.PersonId.HasValue);
            error = null;
            string oldEmail = Context.Login;
            using (var uow = Update())
            {
                if (!BaseSecurity.IsAdminOrTeacher(Context))
                    throw new ChalkableSecurityException(string.Format("User of role {0} can not change password for himself"
                        , Context.Role.Name));

                if (oldEmail != email)
                {
                    var newUser = ServiceLocator.ServiceLocatorMaster.UserService.GetByLogin(email);
                    if (newUser != null && Context.UserId != newUser.Id)
                        error = "User with such email already exists in chalkable";
                    else
                    {
                        ServiceLocator.ServiceLocatorMaster.UserService.ChangeUserLogin(Context.UserId, email);
                        var stiPersonEmail = new StiPersonEmail
                            {
                                Description = "",
                                EmailAddress = email,
                                IsListed = true,
                                IsPrimary = true,
                                PersonId = Context.PersonId.Value
                            };
                        ConnectorLocator.UsersConnector.UpdatePrimaryPersonEmail(stiPersonEmail.PersonId, stiPersonEmail);
                        var person = GetPerson(Context.PersonId.Value);
                        ServiceLocator.ServiceLocatorMaster.EmailService.SendChangedEmailToPerson(person, oldEmail, email);
                    }
                }
                uow.Commit();
            }
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
                //person.FirstLoginDate = Context.NowSchoolTime;
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

        public PaginatedList<Person> SearchPersons(string filter, bool orderByFirstName, int start, int count)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Read())
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                return da.SearchPersons(Context.SchoolLocalId.Value, filter, orderByFirstName, start, count);
            }
        }

        public void ProcessPersonFirstLogin(int id)
        {
            if (Context.PersonId != id)
                throw new ChalkableSecurityException();
            DoUpdate(uow =>
            {
                var da = new PersonDataAccess(uow, Context.SchoolLocalId);
                var p = da.GetById(id);
                if (p.FirstLoginDate.HasValue) return;
                p.FirstLoginDate = Context.NowSchoolTime;
                da.Update(p);
                ServiceLocator.AnnouncementService.SetAnnouncementsAsComplete(Context.NowSchoolTime, true);
            });
        }

        public void UpdateForImport(IList<Person> persons)
        {
            DoUpdate(uow => new PersonDataAccess(uow, Context.SchoolLocalId).UpdateForImport(persons));
        }
        
    }
}
