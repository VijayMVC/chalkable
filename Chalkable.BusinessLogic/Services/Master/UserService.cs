using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        User Login(string login, string password);
        User GetByLogin(string login);
        User GetById(Guid id);
        User CreateSysAdmin(string login, string password);
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

        public User Login(string login, string password)
        {
            using (var uow = Read())
            {
                var da = new UserDataAccess(uow);
                var res = da.GetUser(login, PasswordMd5(password), null);
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
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                User user = new User
                    {
                        Id = Guid.NewGuid(),
                        IsDeveloper = false,
                        IsSysAdmin = true,
                        Login = login,
                        Password = PasswordMd5(password)
                    };
                da.Create(user);
                uow.Commit();
                return user;
            }
        }

        public void ChangePassword(string login, string newPassword)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}