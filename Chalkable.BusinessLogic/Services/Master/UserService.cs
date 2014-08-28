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
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IUserService
    {
        UserContext Login(string login, string password);
        UserContext Login(string login, string password, out string error);
        UserContext Login(string confirmationKey);
        UserContext LoginToDemo(string roleName, string demoPrefix);
        UserContext DeveloperTestLogin(Developer developer);
        UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null);
        UserContext ReLogin(Guid id);
        User GetByLogin(string login);
        User GetById(Guid id);
        IList<User> GetByDistrict(Guid districtId);
        User CreateSysAdmin(string login, string password);
        User CreateDeveloperUser(string login, string password);
        User CreateSchoolUser(string login, string password, Guid? districtId, int? localId, string sisUserName);
        void AssignUserToSchool(IList<SchoolUser> schoolUsers);
        void ChangePassword(string login, string newPassword);
        void ChangeUserLogin(Guid id, string login);
        bool ResetPassword(string email);
        User GetSysAdmin();
        void CreateSchoolUsers(IList<User> userInfos);
        void DeleteUsers(IList<int> localIds, Guid districtId);
        void Edit(IList<User> users);
        void UpdateSisUserNames(List<Pair<string, string>> values);
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
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                da.Update(users);
                uow.Commit();
            }
        }

        public void UpdateSisUserNames(List<Pair<string, string>> values)
        {
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                da.UpdateSisUserNames(values);
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

            if (!user.IsSisUser)
                return SimpleUserLogin(user, password);
            else
            {
                user.OriginalPassword = password;
                using (var uow = Update(IsolationLevel.ReadUncommitted))
                {
                    var res = SisUserLogin(user, uow);
                    uow.Commit();
                    return res;
                }
            }

        }
        public UserContext Login(string confirmationKey)
        {
            using (var uow = Update())
            {
                var da = new UserDataAccess(uow);
                var user = da.GetUser(confirmationKey);
                var districtId = user.DistrictRef;
                if (user.IsSchoolUser)
                    throw new NotImplementedException();
                var res = SimpleUserLogin(user, null);
                if (res != null)
                {
                    user.ConfirmationKey = null;
                    user.DistrictRef = districtId;
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
                var res = DemoUserLogin(demoUser, uow, DemoSchoolConstants.CurrentSchoolYearId);
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
                    UserContext res;
                    if (string.IsNullOrEmpty(user.SisUserName))
                        res = SimpleUserLogin(user, null);
                    else
                    {
                        if(!Context.DistrictId.HasValue)
                            throw new UnassignedUserException();
                        if(string.IsNullOrEmpty(Context.SisToken) || !Context.SisTokenExpires.HasValue)
                            throw new ChalkableException("Can't relogin unlogged user");
                        var district = ServiceLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
                        if (!district.SisDistrictId.HasValue)
                            throw new ChalkableException("There are no such district in Inow");
                        res = SisLogIn(district.SisDistrictId.Value, Context.SisToken, Context.SisTokenExpires.Value, Context.SchoolYearId);
                    }
                    uow.Commit();
                    return res;
                }
            }
            return null;
        }


        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? acadSessionId = null)
        {
            using (var uow = Update(IsolationLevel.ReadUncommitted))
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
                return new UserContext(user, CoreRoles.SUPER_ADMIN_ROLE, user.District, null, null);
            if (user.IsDeveloper)
                return DeveloperLogin(user);
            throw new UnknownRoleException();
        }

        private UserContext SisUserLogin(User user, UnitOfWork uow, ConnectorLocator iNowConnector = null
                                         , StiConnector.Connectors.Model.User iNowUser = null, int? schoolYearId = null)
        {
            if (user == null) return null;
            if (user.IsSchoolUser && user.DistrictRef.HasValue)
            {
                try
                {
                    user = SaveSisToken(user, uow, ref iNowConnector);
                }
                catch (HttpException)
                {
                    return null;
                }

                var schoolL = ServiceLocator.SchoolServiceLocator(user.DistrictRef.Value, null);
                Data.School.Model.SchoolYear schoolYear;
                SchoolUser schoolUser;
                PrepareSchoolData(schoolL, user, schoolYearId, out schoolYear, out schoolUser);
                if (!schoolUser.School.IsChalkableEnabled)
                    return null;
                if (iNowUser == null && iNowConnector != null)
                    iNowUser = iNowConnector.UsersConnector.GetMe();
                if (schoolUser.Role == CoreRoles.TEACHER_ROLE.Id && iNowUser.Claims.All(x => x.Values.All(y => y != "Access Chalkable")))
                    return null;
                var res = new UserContext(user, CoreRoles.GetById(schoolUser.Role), user.District, schoolUser.School, null, schoolYear);
                res.Claims = ClaimInfo.Create(iNowUser.Claims);
                return res;
            }
            throw new UnknownRoleException();
        }

        private UserContext DemoUserLogin(User user, UnitOfWork uow, int? schoolYearId = null)
        {
            if (user == null) return null;
            if (!user.IsSchoolUser || !user.District.IsDemoDistrict) 
                throw new UnknownRoleException();

            Guid? developerId = null;
            var developer = new DeveloperDataAccess(uow).GetDeveloper(user.District.Id);
            if (developer != null) developerId = developer.Id;
            SchoolUser schoolUser;
            Data.School.Model.SchoolYear schoolYear;
            var schoolL = ServiceLocatorFactory.CreateSchoolLocator(user.SchoolUsers[0]);
            PrepareSchoolData(schoolL, user, schoolYearId, out schoolYear, out schoolUser);
            var res = new UserContext(user, CoreRoles.GetById(schoolUser.Role), user.District, schoolUser.School, developerId, schoolYear)
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
            user.District = DemoDistrictStorage.CreateDemoDistrict(developer.DistrictRef.Value);
            return new UserContext(user, CoreRoles.DEVELOPER_ROLE, user.District, null, developer.Id);
        }

        private void PrepareSchoolData(IServiceLocatorSchool schoolL, User user, int? schoolYearId
            , out Data.School.Model.SchoolYear schoolYear, out SchoolUser schoolUser)
        {
            if (schoolYearId.HasValue)
                schoolYear = schoolL.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
            else
            {
                schoolL.Context.SchoolId = user.SchoolUsers.First().SchoolRef;
                schoolL.Context.SchoolLocalId = user.SchoolUsers.First().School.LocalId;
                schoolYear = schoolL.SchoolYearService.GetCurrentSchoolYear();
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
                    user.SisToken = iNowConnector.Token;
                    user.SisTokenExpires = iNowConnector.TokenExpires;
                    new UserDataAccess(uow).Update(user);
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

        public IList<User> GetByDistrict(Guid districtId)
        {
            using (var uow = Read())
            {
                var da = new UserDataAccess(uow);
                var res = da.GetUsers(districtId);
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

        public User CreateDeveloperUser(string login, string password)
        {
            return CreateUser(login, password, false, true, null, null, null);
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
                else if (user.IsSchoolUser && user.SchoolUsers.Count > 0 && user.LocalId.HasValue)
                {
                    var schoolId = user.SchoolUsers.First().SchoolRef;
                    var person = ServiceLocator.SchoolServiceLocator(schoolId).PersonService.GetPerson(user.LocalId.Value);
                    ServiceLocator.EmailService.SendResettedPasswordToPerson(person, key);
                } else
                    throw new NotImplementedException();

                using (var uow = Update())
                {
                    user.ConfirmationKey = key;
                    new UserDataAccess(uow).Update(user);
                    uow.Commit();
                }
                return true;
            }
            return false;
        }
        private string GenerateConfirmationKey()
        {
            var confirmatioKey = Guid.NewGuid().ToString();
            confirmatioKey = confirmatioKey.Replace("-", "");
            return confirmatioKey;
        }


    }
}