using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        UserContext Login(string login, string password);
        User GetByLogin(string login);
        User GetById(Guid id);
        User CreateSysAdmin(string login, string password);
        User CreateSchoolUser(string login, string password, string schoolId, string role);
        void ChangePassword(string login, string newPassword);
        IList<User> GetUsers();
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
                var da = new UserDataAccess(uow);
                var user = da.GetUser(login, PasswordMd5(password), null);
                Guid? schoolId = null;
                string schoolName = null;
                string schoolServerUrl = null;
                CoreRole role;
                
                if (user.SchoolUsers != null && user.SchoolUsers.Count > 0)
                {
                    if (user.SchoolUsers.Count == 1)
                    {
                        var su = user.SchoolUsers[0];
                        schoolId = su.SchoolRef;
                        schoolName = su.School.Name;
                        schoolServerUrl = su.School.ServerUrl;
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
                
                var res = new UserContext(user.Id, schoolId, user.Login, schoolName, schoolServerUrl, role);
                return res;
            }
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

        private User CreateUser(string login, string password, string schoolId, string role, bool isSysAdmin, bool isDeveloper)
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
                    var schoolUserDa = new SchoolUserDataAccess(uow);
                    var schoolUser = new SchoolUser
                    {
                        Id = Guid.NewGuid(),
                        Role = CoreRoles.GetByName(role).Id,
                        UserRef = user.Id,
                        SchoolRef = Guid.Parse(schoolId)
                    };
                    schoolUserDa.Create(schoolUser);
                }
                uow.Commit();
                return user;
            }
        }

        public User CreateSchoolUser(string login, string password, string schoolId, string role)
        {
            if(!(BaseSecurity.IsSysAdmin(Context) || (BaseSecurity.IsAdminEditor(Context) && Context.SchoolId.ToString() == schoolId)))
                throw new ChalkableSecurityException();

            return CreateUser(login, password, schoolId, role, false, false);
        }
    }
}