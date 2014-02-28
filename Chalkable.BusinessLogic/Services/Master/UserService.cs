using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        UserContext Login(string login, string password);
        UserContext Login(string confirmationKey);
        UserContext LoginToDemo(string roleName, string demoPrefix);
        UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime);
        UserContext ReLogin(Guid id);
        User GetByLogin(string login);
        User GetById(Guid id);
        User CreateSysAdmin(string login, string password);
        User CreateDeveloperUser(string login, string password, Guid districtId);
        User CreateSchoolUser(string login, string password, Guid? districtId, int? localId, string sisUserName);
        void AssignUserToSchool(IList<SchoolUser> schoolUsers);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login);
        User GetSysAdmin();
        void CreateSchoolUsers(IList<User> userInfos);
        string PasswordMd5(string password);
    }

    public class UserService : MasterServiceBase, IUserService
    {
        public UserService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public string PasswordMd5(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var b64 = Convert.ToBase64String(hash);
            return b64;
        }

        public UserContext Login(string login, string password)
        {
            using (var uow = Update())
            {
                var user = new UserDataAccess(uow).GetUser(login, PasswordMd5(password), null);
                if (user != null)
                    user.OriginalPassword = password;
                var res = Login(user, uow);
                uow.Commit();
                return res;
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
            using (var uow = Update())
            {
                var demoUser = GetDemoUser(roleName, demoPrefix);
                if (demoUser != null)
                    demoUser.OriginalPassword = PreferenceService.Get(Preference.DEMO_USER_PASSWORD).Value;
                var res = Login(demoUser, uow);
                uow.Commit();
                return res;
            }
        }
        
        public UserContext ReLogin(Guid id)
        {
            if (Context != null && Context.UserId == id)
            {
                using (var uow = Update())
                {
                    var user = new UserDataAccess(uow).GetUser(null, null, id);
                    var res = Login(user, uow);
                    uow.Commit();
                    return res;
                }
            }
            return null;
        }


        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime)
        {
            using (var uow = Update())
            {
                var district = new DistrictDataAccess(uow)
                    .GetAll(new AndQueryCondition{{District.SIS_DISTRICT_IF_FIELD, sisDistrictId}})
                    .First();
                var sisUrl = district.SisUrl;
                var iNowCl = new ConnectorLocator(token, sisUrl, tokenExpiresTime);
                var iNowUser = iNowCl.UsersConnector.GetMe();
                UserContext res = null;
                if (!string.IsNullOrEmpty(iNowUser.Username))
                {
                    var localId = iNowUser.PersonId ?? iNowUser.StudentId ?? iNowUser.StaffId;
                    var chlkUser = new UserDataAccess(uow).GetUser(new AndQueryCondition
                    {
                        {User.LOCAL_ID, localId},
                        {User.DISTRICT_REF_FIELD, district.Id}
                    });
                    res = Login(chlkUser, uow, iNowCl, iNowUser);
                }
                uow.Commit();
                return res;
            }
        }
        
        private UserContext Login(User user, UnitOfWork uow, ConnectorLocator iNowConnector = null
            , StiConnector.Connectors.Model.User iNowUser = null)
        {
            if (user == null) return null;
            
            if (user.SchoolUsers.Count > 1)
                throw new NotSupportedException(ChlkResources.ERR_MULTIPLE_SCHOOL_ARE_NOT_SUPPORTED);
            if (user.IsSysAdmin)
                return new UserContext(user, CoreRoles.SUPER_ADMIN_ROLE, user.District, null, null);
            if (user.IsDeveloper && user.DistrictRef.HasValue)
            {
                var developer = new DeveloperDataAccess(uow).GetDeveloper(user.DistrictRef.Value);
                var school = ServiceLocator.SchoolService.GetSchools(developer.DistrictRef, 0, 10).OrderBy(x => x.LocalId).First(); //todo rewrite this later
                return new UserContext(user, CoreRoles.DEVELOPER_ROLE, user.District, school, developer.Id);
            }
            if (user.IsSchoolUser)
            {
                var su = user.SchoolUsers[0];
                Guid? developerId = null;
                if (!string.IsNullOrEmpty(user.District.DemoPrefix))
                {
                    var developer = new DeveloperDataAccess(uow).GetDeveloper(su.SchoolRef);
                    if (developer != null) developerId = developer.Id;
                }
                user = SaveSisToken(user, uow, ref iNowConnector);
                var res = new UserContext(user, CoreRoles.GetById(su.Role), user.District, su.School, developerId);
                if (iNowUser == null && iNowConnector != null)
                    iNowUser = iNowConnector.UsersConnector.GetMe();          
                if(iNowUser != null) res.Claims = iNowUser.Claims;
                return res;
            }
            throw new UnknownRoleException();
        }

        private User SaveSisToken(User user, UnitOfWork uow, ref ConnectorLocator iNowConnector)
        {
            if (user.SisUserName != null)
            {
                if (iNowConnector == null)
                {
                    if (user.OriginalPassword == null)
                        throw new ChalkableException(ChlkResources.ERR_SIS_CONNECTION_REQUERED_NOT_ENCRYPED_PASSWORD);
                    iNowConnector = ConnectorLocator.Create(user.SisUserName, user.OriginalPassword, user.District.SisUrl); //"http://localhost/"); //"http://sandbox.sti-k12.com/Chalkable/");//
                }
                if (!string.IsNullOrEmpty(iNowConnector.Token))
                {
                    user.SisToken = iNowConnector.Token;
                    user.SisTokenExpires = iNowConnector.TokenExpires;
                    new UserDataAccess(uow).Update(user);
                }
            }
            return user;
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
            return CreateUser(login, password, true, false, null, null, null);
        }

        public void AssignUserToSchool(IList<SchoolUser> schoolUsers)
        {
            using (var uow = Update())
            {
                var schoolUserDa = new SchoolUserDataAccess(uow);
                schoolUserDa.Insert(schoolUsers);
                uow.Commit();
            }
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

        public void CreateSchoolUsers(IList<User> users)
        {
            using (var uow = Update())
            {
                var userDa = new UserDataAccess(uow);
                userDa.Insert(users);
                uow.Commit();
            }
        }

        private User CreateUser(string login, string password, bool isSysAdmin, bool isDeveloper, Guid? districtId, int? localId, string sisUserName)
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
                    Password = PasswordMd5(password),
                    LocalId = localId,
                    DistrictRef = districtId,
                    SisUserName = sisUserName
                };
                userDa.Insert(user);
                uow.Commit();
                return user;
            }
        }

        public User CreateDeveloperUser(string login, string password, Guid districtId)
        {
            return CreateUser(login, password, false, true, districtId, null, null);
        }

        public User CreateSchoolUser(string login, string password, Guid? districtId, int? localId, string sisUserName)
        {
            if(!UserSecurity.CanCreate(Context, districtId))
                throw new ChalkableSecurityException();

            return CreateUser(login, password, false, false, districtId, localId, sisUserName);
        }

        public void ChangeUserLogin(Guid id, string login)
        {
            //todo : check login existing
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