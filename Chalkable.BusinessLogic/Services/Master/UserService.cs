using System;
using System.Linq;
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
        UserContext LoginToDemo(string roleName, string demoPrefix);
        UserContext ReLogin(Guid id);
        User GetByLogin(string login);
        User GetById(Guid id);
        User CreateSysAdmin(string login, string password);
        User CreateSchoolUser(string login, string password, Guid schoolId, string role, Guid? id = null);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login);
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

        public UserContext LoginToDemo(string roleName, string demoPrefix)
        {
            using (var uow = Read())
            {
                return Login(GetDemoUser(roleName, demoPrefix), uow);
            }
        }
        
        public UserContext ReLogin(Guid id)
        {
            if (Context != null && Context.UserId == id)
            {
                using (var uow = Read())
                {
                    var user = new UserDataAccess(uow).GetUser(null, null, id);
                    return Login(user, uow);
                }
            }
            return null;
        }

        private UserContext Login(User user, UnitOfWork uow)
        {
            if (user == null) return null;
            Guid? schoolId = null;
            Guid? districtId = null;
            string schoolName = null;
            string schoolServerUrl = null;
            string schoolTimeZone = null;
            CoreRole role;
            Guid? developerId = null;

            if (user.District != null)
            {
                if (user.SchoolUsers.Count == 1)
                {
                    var su = user.SchoolUsers[0];
                    schoolId = su.SchoolRef;
                    districtId = user.DistrictRef;
                    schoolName = su.School.Name;
                    schoolServerUrl = user.District.ServerUrl;
                    schoolTimeZone = user.District.TimeZone;
                    role = CoreRoles.GetById(su.Role);
                    if (!string.IsNullOrEmpty(user.District.DemoPrefix))
                    {
                        var developer = new DeveloperDataAccess(uow).GetDeveloper(su.SchoolRef);
                        if (developer != null) developerId = developer.Id;
                    }
                }
                else
                    throw new NotSupportedException("multiple school users are not supported yet");
            }
            else
            {
                if (user.IsSysAdmin)
                    role = CoreRoles.SUPER_ADMIN_ROLE;
                else if (user.IsDeveloper)
                {
                    role = CoreRoles.DEVELOPER_ROLE;
                    var developer = new DeveloperDataAccess(uow).GetDeveloper(user.Id);
                    developerId = developer.Id;
                    var school = ServiceLocator.SchoolService.GetSchools(developer.DistrictRef, 0, 1).First();
                    var district = ServiceLocator.DistrictService.GetByIdOrNull(school.DistrictRef);
                    schoolId = school.Id;
                    districtId = school.DistrictRef;
                    schoolName = school.Name;
                    schoolServerUrl = district.ServerUrl;
                    schoolTimeZone = district.TimeZone;
                }
                else
                    throw new Exception("User's role can not be defined");
            }
            var res = new UserContext(user.Id, districtId, schoolId, user.Login, schoolName, schoolTimeZone, schoolServerUrl, role, developerId, user.LocalId);
            return res;
        }

        private string BuildDemoUserName(string roleName, string prefix)
        {
            return prefix + PreferenceService.Get("demoschool" + roleName.ToLower()).Value;
        }

        private User GetDemoUser(string roleName, string prefix)
        {
            if (roleName == CoreRoles.DEVELOPER_ROLE.LoweredName)
            {
                var districts = ServiceLocator.DistrictService.GetDistricts(false, true);
                var district = districts.FirstOrDefault(x => x.DemoPrefix == prefix);
                if (district != null)
                {
                    var developer = ServiceLocator.DeveloperService.GetDeveloperByDictrict(district.Id);
                    if (developer != null) return developer.User;
                }
            }
            return ServiceLocator.UserService.GetByLogin(BuildDemoUserName(roleName, prefix));
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
            if (BaseSecurity.IsSysAdmin(Context) || Context.Login == login)
            {
                using (var uow = Update())
                {
                    var da = new UserDataAccess(uow);
                    var user = da.GetUser(login, null, null);
                    user.Password = PasswordMd5(newPassword);
                    da.Update(user);
                    uow.Commit();
                }
            }
            else
                throw new ChalkableSecurityException();
        }

        private User CreateUser(string login, string password, Guid? schoolId, string role, bool isSysAdmin, bool isDeveloper, Guid? id = null)
        {
            using (var uow = Update())
            {
                var userDa = new UserDataAccess(uow);
                var user = new User
                {
                    Id = id ?? Guid.NewGuid(),
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

        public User CreateSchoolUser(string login, string password, Guid schoolId, string role, Guid? id)
        {
            if(!UserSecurity.CanCreate(Context, schoolId))
                throw new ChalkableSecurityException();

            return CreateUser(login, password, schoolId, role, false, false, id);
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