using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
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
        UserContext SisLogIn(Guid districtId, string token, DateTime tokenExpiresTime);
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


        public UserContext SisLogIn(Guid districtId, string token, DateTime tokenExpiresTime)
        {
            using (var uow = Update())
            {
                var district = new DistrictDataAccess(uow).GetById(districtId);
                var iNowCl = new ConnectorLocator(token, district.SisUrl, tokenExpiresTime);
                var iNowUser = iNowCl.UsersConnector.GetMe();
                UserContext res = null;
                if (!string.IsNullOrEmpty(iNowUser.Username))
                {
                    var chlkUser = new UserDataAccess(uow).GetUser(new Dictionary<string, object>
                    {
                        {User.SIS_USER_NAME_FIELD, iNowUser.Username},
                        {User.DISTRICT_REF_FIELD, districtId}
                    });
                    res = Login(chlkUser, uow, iNowCl);
                }
                uow.Commit();
                return res;
            }
        }

        private UserContext Login(User user, UnitOfWork uow, ConnectorLocator iNowConnector = null)
        {
            if (user == null) return null;
            Guid? schoolId = null;
            Guid? districtId = null;
            string schoolServerUrl = null;
            string schoolTimeZone = null;
            int? schoolLocalId = null;
            CoreRole role;
            Guid? developerId = null;
            string token = null;
            DateTime? tokenExpires = null;
            string sisUrl = null;

            if (user.District != null && user.DistrictRef.HasValue)
            {
                if (user.SchoolUsers.Count <= 1)
                {
                    if (user.SchoolUsers.Count == 1)
                    {
                        var su = user.SchoolUsers[0];
                        schoolId = su.SchoolRef;
                        districtId = user.DistrictRef;
                        schoolServerUrl = user.District.ServerUrl;
                        schoolTimeZone = user.District.TimeZone;
                        schoolLocalId = su.School.LocalId;
                        role = CoreRoles.GetById(su.Role);
                        if (!string.IsNullOrEmpty(user.District.DemoPrefix))
                        {
                            var developer = new DeveloperDataAccess(uow).GetDeveloper(su.SchoolRef);
                            if (developer != null) developerId = developer.Id;
                        }
                        if (user.SisUserName != null)
                        {
                            if (iNowConnector == null)
                            {
                                if (user.OriginalPassword == null)
                                    throw new ChalkableException("Sis connection requires not encripted password");
                                iNowConnector = ConnectorLocator.Create(user.SisUserName, user.OriginalPassword, user.District.SisUrl);   
                            }
                            token = iNowConnector.Token;
                            tokenExpires = iNowConnector.TokenExpires;
                        }
                        sisUrl = user.District.SisUrl;
                    }
                    else if (user.IsDeveloper)
                    {
                        role = CoreRoles.DEVELOPER_ROLE;
                        var developer = new DeveloperDataAccess(uow).GetDeveloper(user.DistrictRef.Value);
                        developerId = developer.Id;
                        var school = ServiceLocator.SchoolService.GetSchools(developer.DistrictRef, 0, 10).OrderBy(x=>x.LocalId).First(); //todo rewrite this later
                        schoolId = school.Id;
                        schoolLocalId = school.LocalId;
                        districtId = school.DistrictRef;
                        schoolServerUrl = user.District.ServerUrl;
                        schoolTimeZone = user.District.TimeZone;
                    }
                    else
                        throw new Exception("User's role can not be defined");
                }
                else
                    throw new NotSupportedException("multiple school users are not supported yet");
            }
            else
            {
                if (user.IsSysAdmin)
                    role = CoreRoles.SUPER_ADMIN_ROLE;
                else
                    throw new Exception("User's role can not be defined");
            }
            var res = new UserContext(user.Id, districtId, schoolId, user.Login, schoolTimeZone, schoolServerUrl, schoolLocalId, role, developerId, user.LocalId, tokenExpires, sisUrl);
            res.SisToken = token;
            if (!string.IsNullOrEmpty(token))
            {
                user.SisToken = token;
                user.OriginalPassword = null;
                user.SisTokenExpires = tokenExpires;
                new UserDataAccess(uow).Update(user);
            }
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