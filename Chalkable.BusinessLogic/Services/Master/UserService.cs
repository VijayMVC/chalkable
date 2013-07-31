﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        UserContext Login(string login, string password);
        UserContext Login(string confirmationKey);
        User GetByLogin(string login);
        User GetById(Guid id);
        User CreateSysAdmin(string login, string password);
        User CreateSchoolUser(string login, string password, Guid schoolId, string role);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login);
        IList<User> GetUsers();
        User GetSysAdmin();

    }

    public class UserService : MasterServiceBase, IUserService
    {
        public UserService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        private string PasswordMd5(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var b64 = Convert.ToBase64String(hash);
            return b64;
        }

        public UserContext Login(string login, string password)
        {
            using (var uow = Read())
            {
                var user = new UserDataAccess(uow).GetUser(login, PasswordMd5(password), null);
                return Login(user, uow);
            }
        }
        public UserContext Login(string confirmationKey)
        {
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                var user = da.GetUser(confirmationKey);
                var res = Login(user, uow);
                if (res != null)
                {
                    user.ConfirmationKey = null;
                    da.Update(user);
                }
                uow.Commit();
                return res;
            }
        }


        private UserContext Login(User user, UnitOfWork uow)
        {
            if (user == null) return null;
            Guid? schoolId = null;
            string schoolName = null;
            string schoolServerUrl = null;
            string schoolTimeZone = null;
            CoreRole role;

            if (user.SchoolUsers != null && user.SchoolUsers.Count > 0)
            {
                if (user.SchoolUsers.Count == 1)
                {
                    var su = user.SchoolUsers[0];
                    schoolId = su.SchoolRef;
                    schoolName = su.School.Name;
                    schoolServerUrl = su.School.ServerUrl;
                    schoolTimeZone = su.School.TimeZone;
                    role = CoreRoles.GetById(su.Role);
                }
                else
                    throw new NotSupportedException("multiple school users are not supported yet");
            }
            else
            {
                if (user.IsSysAdmin)
                    role = CoreRoles.SUPER_ADMIN_ROLE;
                else if (user.IsDeveloper)
                    role = CoreRoles.DEVELOPER_ROLE;
                else
                    throw new Exception("User's role can not be defined");
            }
            Guid? developerId = null;
            if (schoolId.HasValue)
            {
                var developer = new DeveloperDataAccess(uow).GetDeveloper(schoolId.Value);
                if (developer != null)
                    developerId = developer.Id;
            }
            var res = new UserContext(user.Id, schoolId, user.Login, schoolName, schoolTimeZone, schoolServerUrl, role, developerId);
            return res;
        }

        public User GetByLogin(string login)
        {
            using (var uow = Read())
            {
                var da = new UserDataAccess(uow);
                var res = da.GetUser(login, null, null);
                return res;
            }
        }

        public User GetById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new UserDataAccess(uow);
                var res = da.GetUser(null, null, id);
                return res;
            }
        }

        public User CreateSysAdmin(string login, string password)
        {
            return CreateUser(login, password, null, null, true, false);
        }

        public void ChangePassword(string login, string newPassword)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        private User CreateUser(string login, string password, Guid? schoolId, string role, bool isSysAdmin, bool isDeveloper)
        {
            using (var uow = Update())
            {
                var userDa = new UserDataAccess(uow);
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    IsDeveloper = isDeveloper,
                    IsSysAdmin = isSysAdmin,
                    Login = login,
                    Password = PasswordMd5(password)
                };
                userDa.Create(user);
                if (!(isDeveloper || isSysAdmin))
                {
                    if (!schoolId.HasValue)
                        throw new NullReferenceException();
                    var schoolUserDa = new SchoolUserDataAccess(uow);
                    var schoolUser = new SchoolUser
                    {
                        Id = Guid.NewGuid(),
                        Role = CoreRoles.GetByName(role).Id,
                        UserRef = user.Id,
                        SchoolRef = schoolId.Value
                    };
                    schoolUserDa.Insert(schoolUser);
                }
                uow.Commit();
                return user;
            }
        }

        public User CreateSchoolUser(string login, string password, Guid schoolId, string role)
        {
            if(!UserSecurity.CanCreate(Context, schoolId))
                throw new ChalkableSecurityException();

            return CreateUser(login, password, schoolId, role, false, false);
        }

        public void ChangeUserLogin(Guid id, string login)
        {
            var user = GetById(id);
            if(!UserSecurity.CanModify(Context, user))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                user.Login = login;
                new UserDataAccess(uow).Update(user);
                uow.Commit();
            }
        }


        public User GetSysAdmin()
        {
            using (var uow = Read())
            {
                return new UserDataAccess(uow).GetSysAdmin();
            }
        }

       
    }
}