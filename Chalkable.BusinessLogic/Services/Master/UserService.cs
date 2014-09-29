using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        UserContext Login(string login, string password);
        UserContext Login(string login, string password, out string error);
        UserContext Login(string confirmationKey);
        UserContext DemoLogin(string roleName, string demoPrefix);
        UserContext DeveloperTestLogin(Developer developer);
        UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null);
        User GetByLogin(string login);
        User GetById(Guid id);
        User GetBySisUserId(int userId, Guid? districtId);
        User CreateDeveloperUser(string login, string password);
        void AddSchoolUsers(IList<SchoolUser> schoolUsers);
        void DeleteSchoolUsers(IList<SchoolUser> schoolUsers);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login);
        bool ResetPassword(string email);
        User GetSysAdmin();
        void DeleteUsers(IList<int> localIds, Guid districtId);
        void ImportEdit(IList<User> users);
        void Add(IList<User> users);

    }

    public class UserService : MasterServiceBase, IUserService
    {
        public UserService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public static string PasswordMd5(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            var b64 = Convert.ToBase64String(hash);
            return b64;
        }

        public void ImportEdit(IList<User> users)
        {
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                da.UpdateUsersForImport(users);
                uow.Commit();
            }
        }

        public void Add(IList<User> users)
        {
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                da.Insert(users);
                var loginInfos = users.Where(x=>x.LoginInfo != null).Select(x => x.LoginInfo).ToList();
                if (loginInfos.Count > 0)
                    new UserLoginInfoDataAccess(uow).Insert(loginInfos);
                uow.Commit();
            }
        }

        public UserContext Login(string login, string password)
        {
            string error = null;
            var res = Login(login, password, out error);
            return res;
        }

        public UserContext Login(string login, string password, out string error)
        {
            var user = GetByLogin(login);
            error = null;
            if (user == null)
            {
                error = "Email not recognized";
                return null;
            }
            if (!user.SisUserId.HasValue)
                return SimpleUserLogin(user, password);
            user.OriginalPassword = password;
            using (var uow = Update(IsolationLevel.ReadUncommitted))
            {
                var res = SisUserLogin(user, uow);
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
                var districtId = user.DistrictRef;
                if (user.SisUserId.HasValue)
                    throw new NotImplementedException();
                var res = SimpleUserLogin(user, null);
                if (res != null)
                {
                    user.ConfirmationKey = null;
                    user.DistrictRef = districtId;
                    da.Update(user);
                    UpdateUserLoginInfo(user, null, null, null, uow);
                }
                uow.Commit();
                return res;
            }
        }

        public UserContext DemoLogin(string roleName, string demoPrefix)
        {
            using (var uow = Update())
            {
                var demoUser = GetDemoUser(roleName, demoPrefix);
                if (demoUser != null)
                    demoUser.OriginalPassword = PreferenceService.Get(Preference.DEMO_USER_PASSWORD).Value;
                var res = DemoUserLogin(demoUser, uow, DemoSchoolConstants.CurrentSchoolYearId);
                uow.Commit();
                return res;
            }
        }

        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null)
        {
            using (var uow = Update(IsolationLevel.ReadUncommitted))
            {
                var district = new DistrictDataAccess(uow)
                    .GetAll(new AndQueryCondition{{District.ID_FIELD, sisDistrictId}})
                    .First();
                var sisUrl = district.SisUrl;
                var iNowCl = new ConnectorLocator(token, sisUrl, tokenExpiresTime);
                var iNowUser = iNowCl.UsersConnector.GetMe();
                UserContext res = null;
                if (!string.IsNullOrEmpty(iNowUser.Username))
                {
                    var chlkUser = new UserDataAccess(uow).GetUser(new AndQueryCondition
                    {
                        {User.SIS_USER_ID_FIELD, iNowUser.Id},
                        {User.DISTRICT_REF_FIELD, district.Id}
                    });
                    res = SisUserLogin(chlkUser, uow, iNowCl, iNowUser, acadSessionId);
                }
                uow.Commit();
                return res;
            }
        }      

        private UserContext SimpleUserLogin(User user,  string password)
        {
            if (user == null) return null;
            if (!string.IsNullOrEmpty(password) && user.Password != PasswordMd5(password)) return null;
            if (user.IsSysAdmin)
                return new UserContext(user, CoreRoles.SUPER_ADMIN_ROLE, user.District, null, null, null);
            if (user.IsDeveloper)
                return DeveloperLogin(user);
            throw new UnknownRoleException();
        }

        private UserContext SisUserLogin(User user, UnitOfWork uow, ConnectorLocator iNowConnector = null
                                         , StiConnector.Connectors.Model.User iNowUser = null, int? schoolYearId = null)
        {
            if (user == null) return null;
            if (user.SisUserId.HasValue)
            {
                try
                {
                    user = SaveSisToken(user, uow, ref iNowConnector);
                }
                catch (HttpException ex)
                {
                    return null;
                }

                var schoolL = ServiceLocator.SchoolServiceLocator(user.DistrictRef.Value, null);
                Data.School.Model.SchoolYear schoolYear;
                SchoolUser schoolUser;
                var userAcadSessionsIds = iNowConnector.UsersConnector.GetUserAcadSessionsIds();
                if(userAcadSessionsIds.Length == 0)
                    throw new ChalkableException("Current user does not have access to any of school acadSessions");
                if (!schoolYearId.HasValue && userAcadSessionsIds.Length == 1)
                    schoolYearId = userAcadSessionsIds[0];
                PrepareSchoolData(schoolL, user, schoolYearId, userAcadSessionsIds, out schoolYear, out schoolUser);
                if (!schoolUser.School.IsChalkableEnabled)
                    return null;
                if (iNowUser == null)
                    iNowUser = iNowConnector.UsersConnector.GetMe();
                int roleId;
                int personId = PersonDataAccess.GetPersonDataForLogin(user.District.ServerUrl, user.DistrictRef.Value,
                                                                      user.SisUserId.Value, out roleId);
                if (roleId == CoreRoles.TEACHER_ROLE.Id && iNowUser.Claims.All(x => x.Values.All(y => y != "Access Chalkable")))
                    return null;
                var res = new UserContext(user, CoreRoles.GetById(roleId), user.District, schoolUser.School, null, personId, schoolYear);
                
                res.Claims = ClaimInfo.Create(iNowUser.Claims);
                return res;
            }
            throw new UnknownRoleException();
        }

        private UserContext DemoUserLogin(User user, UnitOfWork uow, int? schoolYearId = null)
        {
            if (user == null) return null;
            if (!user.District.IsDemoDistrict) 
                throw new ChalkableException("This login is allowed to demo district only");

            Guid? developerId = null;
            var developer = new DeveloperDataAccess(uow).GetDeveloper(user.District.Id);
            if (developer != null) developerId = developer.Id;
            SchoolUser schoolUser;
            Data.School.Model.SchoolYear schoolYear;
            var schoolL = ServiceLocatorFactory.CreateSchoolLocator(user.SchoolUsers[0]);
            PrepareSchoolData(schoolL, user, schoolYearId, null, out schoolYear, out schoolUser);

            int roleId;
            int personId = DemoPersonStorage.GetPersonDataForLogin(schoolUser.User, out roleId);
            var res = new UserContext(user, CoreRoles.GetById(roleId), user.District, schoolUser.School, developerId, personId, schoolYear)
            {
                Claims = ClaimInfo.Create(DemoUserService.GetDemoClaims())
            };
            return res;
        }

        private UserContext DeveloperLogin(User user)
        {
            var developer = ServiceLocator.DeveloperService.GetDeveloperById(user.Id);
            return GetDeveloperContext(developer);
        }

        public UserContext DeveloperTestLogin(Developer developer)
        {
            return GetDeveloperContext(developer);
        }

        private UserContext GetDeveloperContext(Developer developer)
        {
            var user = developer.User;
            user.DistrictRef = developer.DistrictRef;
            user.LoginInfo = new UserLoginInfo() {Id = user.Id};
            user.District = DemoDistrictStorage.CreateDemoDistrict(developer.DistrictRef.Value);
            return new UserContext(user, CoreRoles.DEVELOPER_ROLE, user.District, null, developer.Id, null);
        }

        private void PrepareSchoolData(IServiceLocatorSchool schoolL, User user, int? schoolYearId, int[] acdaIds
            , out Data.School.Model.SchoolYear schoolYear, out SchoolUser schoolUser)
        {
            if (schoolYearId.HasValue)
                schoolYear = schoolL.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
            else
            {
                schoolL.Context.SchoolLocalId = user.SchoolUsers.First().SchoolRef;
                schoolL.Context.SchoolLocalId = user.SchoolUsers.First().School.LocalId;
                schoolYear = schoolL.SchoolYearService.GetCurrentSchoolYear();
                if (acdaIds != null && !acdaIds.Contains(schoolYear.Id))
                {
                    var schoolYears = schoolL.SchoolYearService.GetSortedYears();
                    schoolYears = schoolYears.Where(x => acdaIds.Contains(x.Id))
                        .OrderByDescending(x=>x.StartDate).ToList();
                    if(schoolYears.Count == 0)
                        throw new ChalkableException("Current user doesn't have access to acada sessions in current school");
                    schoolYear = schoolYears[0];
                }
            }
            var schoolId = schoolYear.SchoolRef;
            schoolUser = user.SchoolUsers.FirstOrDefault(x => x.School.LocalId == schoolId);
            if (schoolUser == null)
                throw new ChalkableException(string.Format("There is no school in current District with such schoolYearId : {0}", schoolYear.Id));    
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
                    UpdateUserLoginInfo(user, iNowConnector.Token, iNowConnector.TokenExpires, null, uow);
                }
            }
            return user;
        }

        private User GetDemoUser(string roleName, string prefix)
        {
            return DemoUserService.GetDemoUser(roleName, prefix);
        }

        public User GetByLogin(string login)
        {
            if (DemoUserService.IsDemoUser(login))
                return DemoUserService.GetDemoUser(login);
            
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

        public void AddSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            using (var uow = Update())
            {
                var schoolUserDa = new SchoolUserDataAccess(uow);
                schoolUserDa.Insert(schoolUsers);
                uow.Commit();
            }
        }

        public void DeleteSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            using (var uow = Update())
            {
                var schoolUserDa = new SchoolUserDataAccess(uow);
                schoolUserDa.Delete(schoolUsers);
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
                    UpdateUserLoginInfo(user, null, null, Context.NowSchoolTime, uow);
                    uow.Commit();
                }
            }
            else
                throw new ChalkableSecurityException();
        }

        
        public User CreateDeveloperUser(string login, string password)
        {
            using (var uow = Update())
            {
                var userDa = new UserDataAccess(uow);
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    IsDeveloper = true,
                    IsSysAdmin = false,
                    Login = login,
                    Password = PasswordMd5(password)
                };
                userDa.Insert(user);
                user.LoginInfo = new UserLoginInfo {Id = user.Id};
                new UserLoginInfoDataAccess(uow).Insert(user.LoginInfo);
                uow.Commit();
                return user;
            }
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

        public void DeleteUsers(IList<int> localIds, Guid districtId)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new UserDataAccess(uow).Delete(localIds, districtId);
                uow.Commit();
            }
        }


        public bool ResetPassword(string email)
        {
            var user = GetByLogin(email);
            if (user != null)
            {
                var key = GenerateConfirmationKey();
                if (user.IsDeveloper)
                {
                    var developer = ServiceLocator.DeveloperService.GetDeveloperById(user.Id);
                    ServiceLocator.EmailService.SendResettedPasswordToDeveloper(developer, key);
                }
                else if (user.SisUserId.HasValue)
                    ServiceLocator.EmailService.SendResettedPasswordToPerson(user, key);
                else
                    throw new NotImplementedException();

                using (var uow = Update())
                {
                    user.ConfirmationKey = key;
                    new UserDataAccess(uow).Update(user);
                    UpdateUserLoginInfo(user, null, null, Context.NowSchoolTime, uow);
                    uow.Commit();
                }
                return true;
            }
            return false;
        }

        private void UpdateUserLoginInfo(User user, string sisToken, DateTime? sisTokenExpires, DateTime? lastPasswordReset, UnitOfWork uow)
        {
            Action<UserLoginInfo> action;
            if (user.LoginInfo == null)
            {
                user.LoginInfo = new UserLoginInfo { Id = user.Id };
                action = new UserLoginInfoDataAccess(uow).Insert;
            }
            else action = new UserLoginInfoDataAccess(uow).Update;

            user.LoginInfo.LastPasswordReset = lastPasswordReset;
            user.LoginInfo.SisToken = sisToken;
            user.LoginInfo.SisTokenExpires = sisTokenExpires;
            action(user.LoginInfo);
        }

        private string GenerateConfirmationKey()
        {
            var confirmatioKey = Guid.NewGuid().ToString();
            confirmatioKey = confirmatioKey.Replace("-", "");
            return confirmatioKey;
        }


        public User GetBySisUserId(int userId, Guid? districtId)
        {
            using (var uow = Read())
            {
                var conds = new AndQueryCondition {{User.SIS_USER_ID_FIELD, userId}};
                if(districtId.HasValue)
                    conds.Add(User.DISTRICT_REF_FIELD, districtId.Value);
                return new UserDataAccess(uow).GetUser(conds);
            }
        }
    }
}