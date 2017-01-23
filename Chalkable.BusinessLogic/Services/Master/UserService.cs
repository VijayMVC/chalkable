using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
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
        UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null, string sisRedirectUrl = null);
        UserContext SwitchToRole(CoreRole role);
        User GetByLogin(string login);
        User GetById(Guid id);
        User GetBySisUserId(int userId, Guid? districtId);
        User CreateDeveloperUser(string login, string password);
        string GetUserEmailById(Guid id);
        void AddSchoolUsers(IList<SchoolUser> schoolUsers);
        void DeleteSchoolUsers(IList<SchoolUser> schoolUsers);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login, out string error);
        bool CanChangeUserLogin(Guid userId);
        bool ResetPassword(string email);
        User GetSysAdmin();
        void DeleteUsers(IList<int> localIds, Guid districtId);
        void Edit(IList<User> users);
        void Add(IList<User> users);
        void CreateUserLoginInfos(IList<Guid> ids);
        IList<User> GetAll(Guid districtId);
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

        public void Edit(IList<User> users)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new UserDataAccess(u).UpdateUsersForImport(users));
        }

        public void Add(IList<User> users)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u =>
            {
                new UserDataAccess(u).Insert(users);
                var loginInfos = users.Where(x => x.LoginInfo != null).Select(x => x.LoginInfo).ToList();
                if (loginInfos.Count > 0)
                    new UserLoginInfoDataAccess(u).Insert(loginInfos);
            });
        }

        public void CreateUserLoginInfos(IList<Guid> ids)
        {
            DoUpdate(u => new UserDataAccess(u).CreateUserLoginInfos(ids, 30 + ids.Count));
        }

        public UserContext Login(string login, string password)
        {
            string error;
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
                var context = SisUserLogin(user, uow);
                uow.Commit();
                return context;
            }
        }

        public UserContext Login(string confirmationKey)
        {
            if (string.IsNullOrEmpty(confirmationKey)) return null;
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                var user = da.GetUser(confirmationKey);
                if (user == null) return null;
                
                var districtId = user.DistrictRef;
                if (user.SisUserId.HasValue)
                    throw new NotImplementedException();
                var res = SimpleUserLogin(user);
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

        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null, string sisRedirectUrl = null)
        {
            using (var uow = Update(IsolationLevel.ReadUncommitted))
            {
                var district = new DistrictDataAccess(uow).GetById(sisDistrictId);
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
                    res = SisUserLogin(chlkUser, uow, iNowCl, iNowUser, acadSessionId, sisRedirectUrl);
                }
                uow.Commit();
                return res;
            }
        }      

        private UserContext SimpleUserLogin(User user,  string password)
        {
            if (user == null || string.IsNullOrEmpty(password) || user.Password != PasswordMd5(password)) 
                return null;
            return SimpleUserLogin(user);
        }

        private UserContext SimpleUserLogin(User user)
        {
            if (user == null) 
                return null;
            if (user.IsSysAdmin)
                return new UserContext(user, CoreRoles.SUPER_ADMIN_ROLE, user.District, null, null, null, null);
            if (user.IsDistrictRegistrator)
                return new UserContext(user, CoreRoles.DISTRICT_REGISTRATOR_ROLE, user.District, null, null, null, null);
            if (user.IsDeveloper)
                return DeveloperLogin(user);
            if (user.IsAppTester)
                return new UserContext(user, CoreRoles.APP_TESTER_ROLE, user.District, null, null, null, null);
            if (user.IsAssessmentAdmin)
                return new UserContext(user, CoreRoles.ASSESSMENT_ADMIN_ROLE, user.District, null, null, null, null);
            throw new UnknownRoleException();           
        }


        private UserContext SisUserLogin(User user, UnitOfWork uow, ConnectorLocator iNowConnector = null
                                         , StiConnector.Connectors.Model.User iNowUser = null, int? schoolYearId = null, string sisRedirectUrl = null)
        {
            if (user == null) return null;
            if (user.SisUserId.HasValue)
            {
                try
                {
                    SaveSisToken(user, uow, ref iNowConnector);
                }
                catch (HttpException)
                {
                    return null;
                }
                Trace.Assert(user.DistrictRef.HasValue);
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
                int personId = PersonDataAccess.GetPersonDataForLogin(user.District.ServerUrl, user.DistrictRef.Value, user.SisUserId.Value, out roleId);
                var claimInfos = ClaimInfo.Create(iNowUser.Claims);
                if (roleId == CoreRoles.TEACHER_ROLE.Id)
                {
                    EnsureTeacherChalkableAccess(claimInfos);
                }
#if DEBUG
                var loginTimeOut = (int?)null;
#else
                var loginTimeOut = schoolL.AppSettingService.GetLoginTimeOut();
#endif

                var res = new UserContext(user, CoreRoles.GetById(roleId), user.District, schoolUser.School, null, personId, loginTimeOut, schoolYear, sisRedirectUrl)
                {
                    Claims = ClaimInfo.Create(iNowUser.Claims),
                    SisApiVersion = iNowConnector.ApiVersion
                };
                return res;
            }
            throw new UnknownRoleException();
        }

        public UserContext SwitchToRole(CoreRole role)
        {
            if (role == CoreRoles.DISTRICT_ADMIN_ROLE)
            {
                EnsureDistrictAdminAccess(Context.Claims);
                Context.Role = role;
                Context.RoleId = role.Id;
                UpdateContext();
                return Context;
            }
            if (role == CoreRoles.TEACHER_ROLE)
            {
                EnsureTeacherChalkableAccess(Context.Claims);
                Context.Role = role;
                Context.RoleId = role.Id;
                UpdateContext();
                return Context;
            }
            throw new NotImplementedException();
        }

        private void UpdateContext()
        {
            //reset messaging settings 
            if (Context.DistrictId.HasValue)
            {
                var settings = ServiceLocator.SchoolService.GetDistrictMessaginSettings(Context.DistrictId.Value);
                Context.StudentClassMessagingOnly = settings.StudentToClassMessagingOnly;
                Context.StudentMessagingEnabled = settings.StudentMessagingEnabled;
                Context.TeacherStudentMessaginEnabled = settings.TeacherToStudentMessaginEnabled;
                Context.TeacherClassMessagingOnly = settings.TeacherToClassMessagingOnly;
            }
        }

        private void EnsureDistrictAdminAccess(IList<ClaimInfo> claimInfos)
        {
            if (!claimInfos.HasPermission(ClaimInfo.CHALKABLE_ADMIN))
                throw new ChalkableSecurityException(string.Format("User has no required ({0}) permission for using Chalkable {1} Portal", ClaimInfo.CHALKABLE_ADMIN));
        }

        private void EnsureTeacherChalkableAccess(IList<ClaimInfo> claimInfos)
        {
            if (!HasTeacherAccessToChalkable(claimInfos))
                throw new ChalkableException($"User has no required ({ClaimInfo.MAINTAIN_CLASSROOM} , {ClaimInfo.MAINTAIN_CLASSROOM_ADMIN}) permission for using Chalkable {CoreRoles.TEACHER_ROLE.Name} Portal");             
        }

        private bool HasTeacherAccessToChalkable(IList<ClaimInfo> claimInfos)
        {
            return claimInfos.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM) ||
                   claimInfos.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN);
        }

        private UserContext DemoUserLogin(User user, UnitOfWork uow, int? schoolYearId = null)
        {
            if (user == null) return null;
            if (!user.District.IsDemoDistrict) 
                throw new ChalkableException("This login is allowed to demo district only");

            Guid? developerId = null;
            var developer = new DeveloperDataAccess(uow).GetDeveloper(user.District.Id);
            if (developer != null) developerId = developer.Id;
            var schoolUser = user.SchoolUsers.First();
            var schoolYear = DemoSchoolYearService.GetDemoSchoolYear();
            int roleId;
            var personId = DemoPersonService.GetPersonDataForLogin(schoolUser.User, out roleId);
            var res = new UserContext(user, CoreRoles.GetById(roleId), user.District, schoolUser.School, developerId, personId, null, schoolYear)
            {
                Claims = ClaimInfo.Create(DemoUserService.GetDemoClaims())
            };
            return res;
        }

        private UserContext DeveloperLogin(User user)
        {
            var developer = ServiceLocator.DeveloperService.GetById(user.Id);
            return GetDeveloperContext(developer);
        }

        public UserContext DeveloperTestLogin(Developer developer)
        {
            return GetDeveloperContext(developer);
        }

        private UserContext GetDeveloperContext(Developer developer)
        {
            Trace.Assert(developer.DistrictRef.HasValue);
            var user = developer.User;
            user.DistrictRef = developer.DistrictRef;
            user.LoginInfo = new UserLoginInfo {Id = user.Id};
            user.District = DemoDistrictService.CreateDemoDistrict(developer.DistrictRef.Value);
            return new UserContext(user, CoreRoles.DEVELOPER_ROLE, user.District, null, developer.Id, null, null);
        }

        private void PrepareSchoolData(IServiceLocatorSchool schoolL, User user, int? schoolYearId, int[] acdaIds
            , out Data.School.Model.SchoolYear schoolYear, out SchoolUser schoolUser)
        {
            if (schoolYearId.HasValue)
                schoolYear = schoolL.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
            else
            {
                var schoolYears = schoolL.SchoolYearService.GetDescSortedYearsByIds(acdaIds.ToList());
                if(schoolYears.Count == 0)
                    throw new ChalkableException("Current user doesn't have access to acada sessions in current school");

                schoolYear = (schoolYears.FirstOrDefault(x => (x.EndDate >= DateTime.Now && x.StartDate <= DateTime.Now)) ??
                              schoolYears.FirstOrDefault(x => x.EndDate <= DateTime.Now)) ?? schoolYears.Last();
            }
            var schoolId = schoolYear.SchoolRef;
            schoolUser = user.SchoolUsers.FirstOrDefault(x => x.School.LocalId == schoolId);
            if (schoolUser == null)
                throw new ChalkableException($"There is no school in current District with such schoolYearId : {schoolYear.Id}");    
        }
        
        private void SaveSisToken(User user, UnitOfWork uow, ref ConnectorLocator iNowConnector)
        {
            if (user.SisUserName != null)
            {
                if (iNowConnector == null)
                {
                    if (user.OriginalPassword == null)
                        throw new ChalkableException(ChlkResources.ERR_SIS_CONNECTION_REQUERED_NOT_ENCRYPED_PASSWORD);
                    iNowConnector = ConnectorLocator.Create(user.SisUserName, user.OriginalPassword, user.District.SisUrl);
                }
                if (!string.IsNullOrEmpty(iNowConnector.Token))
                {
                    UpdateUserLoginInfo(user, iNowConnector.Token, iNowConnector.TokenExpires, null, uow);
                }
            }
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
            return DoRead(u => new UserDataAccess(u).GetUser(null, null, id));
        }

        public void AddSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            DoUpdate(u => new DataAccessBase<SchoolUser, Guid>(u).Insert(schoolUsers));
        }

        public void DeleteSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            DoUpdate(u => new DataAccessBase<SchoolUser, Guid>(u).Delete(schoolUsers));
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

        public void ChangeUserLogin(Guid userId, string login, out string error)
        {
            //todo : check login existing
            error = null;
            var user = GetById(userId);

            if(!CanChangeUserLogin(user))
                throw new ChalkableSecurityException();

            if (user.Login != login)
            {
                var newUser = GetByLogin(login);
                if (newUser != null && userId != newUser.Id)
                {
                    error = "User with such login already exists in chalkable";
                    return;
                }
                
                DoUpdate(u =>
                {
                    user.Login = login;
                    new UserDataAccess(u).Update(user);
                });
            }
        }

        public bool CanChangeUserLogin(Guid userId)
        {
            var user = GetById(userId);
            return CanChangeUserLogin(user);
        }

        private bool CanChangeUserLogin(User user)
        {
            var res = UserSecurity.CanModify(Context, user);
            if (!res && Context.DistrictId.HasValue && user.SisUserId.HasValue && BaseSecurity.IsTeacher(Context))
            {
                int roleId;
                var personId = PersonDataAccess.GetPersonDataForLogin(Context.DistrictServerUrl, Context.DistrictId.Value, user.SisUserId.Value, out roleId);
                if (roleId == CoreRoles.STUDENT_ROLE.Id)
                {
                    var schooLocator = ServiceLocator.SchoolServiceLocator(Context.DistrictId.Value, Context.SchoolLocalId);
                    res = (Context.PersonId.HasValue && Context.SchoolYearId.HasValue 
                           && schooLocator.StudentService.IsTeacherStudent(Context.PersonId.Value, personId, Context.SchoolYearId.Value));
                }

            }
            return res;
        }


        public User GetSysAdmin()
        {
            return DoRead(u => new UserDataAccess(u).GetSysAdmin());
        }

        public void DeleteUsers(IList<int> localIds, Guid districtId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
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
                    var developer = ServiceLocator.DeveloperService.GetById(user.Id);
                    ServiceLocator.EmailService.SendResettedPasswordToDeveloper(developer, key);
                }else if (user.IsSysAdmin || user.IsAppTester || user.IsAssessmentAdmin)
                    ServiceLocator.EmailService.SendResettedPasswordToPerson(user, key);
                else if (user.SisUserId.HasValue)
                    throw new ChalkableException($@"Please use <a href=""{user.District.SisRedirectUrl}"" target=""_blank"">InformationNOW</a> to reset your password");
                else
                    throw new ChalkableException("Please contact system administrator to reset your password");

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

        public IList<User> GetAll(Guid districtId)
        {
            using (var uow = Read())
            {
                var conds = new AndQueryCondition { { User.DISTRICT_REF_FIELD, districtId } };                
                return new UserDataAccess(uow).GetAll(conds);
            }
        }

        public string GetUserEmailById(Guid id)
        {
            return GetById(id).Login;
        }
    }
}