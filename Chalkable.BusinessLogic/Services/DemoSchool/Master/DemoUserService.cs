using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors.Model;
using User = Chalkable.Data.Master.Model.User;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{
    public class DemoUserStorage : BaseDemoGuidStorage<User>
    {
        public DemoUserStorage()
            : base(x => x.Id, false)
        {
        }
    }

    public class DemoUserService : DemoMasterServiceBase, IUserService
    {
        private DemoUserStorage UserStorage { get; set; }
        public DemoUserService(IServiceLocatorMaster serviceLocator)
            : base(serviceLocator)
        {
        }

        public string PasswordMd5(string password)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<User> users)
        {
            throw new NotImplementedException();
        }

        public void UpdateSisUserNames(List<Pair<string, string>> values, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public User GetByLocalId(int localId, Guid districtId)
        {
            throw new NotImplementedException();
        }

        public void Add(IList<User> users)
        {
            UserStorage.Add(users);
        }

        public void CreateUserLoginInfos(IList<Guid> ids)
        {
            throw new NotImplementedException();
        }

        private const string DEMO_USER_PREFIX = "demo_user_";

        private static bool IsDemoLogin(string userLogin)
        {
            var logins = new[]
            {
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_EDIT).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_GRADE).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_VIEW).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_TEACHER).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_STUDENT).Value
            };

            return logins.Any(login => String.Equals(userLogin, login, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool IsDemoUser(UserContext context)
        {
            var userLogin = context.DistrictId.HasValue ? context.Login.Replace(context.DistrictId.ToString(), "") : context.Login;
            return IsDemoLogin(userLogin);
        }


        public static User GetDemoUser(string login)
        {
            var userRoles = new Dictionary<string, string>
            {
                {PreferenceService.Get("demoschool" + CoreRoles.TEACHER_ROLE.LoweredName).Value, CoreRoles.TEACHER_ROLE.LoweredName},
                {PreferenceService.Get("demoschool" + CoreRoles.STUDENT_ROLE.LoweredName).Value,CoreRoles.STUDENT_ROLE.LoweredName}
            };

            var userLogin = login.Substring(login.IndexOf(DEMO_USER_PREFIX, StringComparison.InvariantCultureIgnoreCase));
            var prefix = login.Substring(0, login.IndexOf(DEMO_USER_PREFIX, StringComparison.InvariantCultureIgnoreCase));

            if (userRoles.ContainsKey(userLogin))
            {
                var role = userRoles[userLogin];
                return GetDemoUser(role, prefix);
            }
            return null;
        }

        public static bool IsDemoUser(string login)
        {
            var index = login.IndexOf(DEMO_USER_PREFIX, StringComparison.InvariantCultureIgnoreCase);
            return IsDemoLogin(index != -1 ? login.Substring(index) : login);
        }


        public static User CreateDemoUser(Guid districtRef, Guid userId, string login)
        {
            var schoolUsers = new List<SchoolUser>();

            var school = DemoSchoolService.CreateMasterSchool(districtRef);

            var district = DemoDistrictService.CreateDemoDistrict(districtRef);

            var user = new User
            {
                ConfirmationKey = null,
                DistrictRef = districtRef,
                Id = userId,
                IsDeveloper = false,
                IsSysAdmin = false,
                Login = login,
                IsDemoUser = true,
                District = district,
                LoginInfo = new UserLoginInfo
                {
                    Id = userId
                }
            };

            schoolUsers.Add(new SchoolUser
            {
                School = school,
                User = user
            });

            user.SchoolUsers = schoolUsers;

            return user;
        }

        public static User GetDemoUser(string roleName, string prefix)
        {
            var demoUserName = BuildDemoUserName(roleName, prefix);
            var userRef = Guid.NewGuid();
            return CreateDemoUser(Guid.Parse(prefix), userRef, demoUserName);
        }

        public UserContext Login(string login, string password)
        {
            string error;
            return Login(login, password, out error);
        }

        public UserContext Login(string login, string password, out string error)
        {
            error = null;
            return new UserContext();
        }
        public UserContext Login(string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public UserContext DemoLogin(string roleName, string demoPrefix)
        {
            throw new NotImplementedException();
        }

        public UserContext DeveloperTestLogin(Developer developer)
        {
            throw new NotImplementedException();
        }

        public UserContext ReLogin(Guid id)
        {
            throw new NotImplementedException();
        }

        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime, int? schoolYearId, string sisRedirectUrl = null)
        {
            throw new NotImplementedException();
        }

        public UserContext SwitchToRole(CoreRole role)
        {
            throw new NotImplementedException();
        }

        public static string BuildDemoUserName(string roleName, string prefix)
        {
            return prefix + PreferenceService.Get("demoschool" + roleName.ToLower()).Value;
        }

        public User GetByLogin(string login)
        {
            throw new NotImplementedException();
        }

        public User GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public User GetBySisUserId(int userId, Guid? districtId)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetByDistrict(Guid districtId)
        {
            throw new NotImplementedException();
        }

        public User CreateSysAdmin(string login, string password)
        {
            throw new NotImplementedException();
        }

        public void AddSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            throw new NotImplementedException();
        }

        public void DeleteSchoolUsers(IList<SchoolUser> schoolUsers)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string login, string newPassword)
        {
        }


        public User CreateDeveloperUser(string login, string password)
        {
            throw new NotImplementedException();
        }

        public User CreateSchoolUser(string login, string password, Guid? districtId, int? localId, string sisUserName)
        {
            throw new NotImplementedException();
        }

        public void ChangeUserLogin(Guid id, string login, out string error)
        {
            throw new NotImplementedException();
        }

        public bool CanChangeUserLogin(Guid userId)
        {
            throw new NotImplementedException();
        }

        public bool ResetPassword(string email)
        {
            throw new NotImplementedException();
        }


        public User GetSysAdmin()
        {
            throw new NotImplementedException();
        }

        public void DeleteUsers(IList<int> localIds, Guid districtId)
        {
            throw new NotImplementedException();
        }


        public static IList<Claim> GetDemoClaims()
        {
            return new List<Claim>
            {
                new Claim
                {
                    Type = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    Values = new List<string>
                    {
                        "Access Future Academic Sessions",
                        "Access Past Academic Sessions",
                        "View Academic Session",
                        "Maintain Address",
                        "View Address",
                        "Maintain Attendance",
                        "View Attendance",
                        "Maintain Classroom Absence Reasons",
                        "Maintain Classroom Attendance",
                        "Maintain Classroom Discipline",
                        "Maintain Classroom Discipline (Admin)",
                        "Maintain Classroom Grades",
                        "Maintain Classroom Lunch Count",
                        "Maintain Classroom Roster",
                        "Repost Classroom Attendance",
                        "View Classroom Absence Reasons",
                        "View Classroom Attendance",
                        "View Classroom Attendance (Admin)",
                        "View Classroom Discipline",
                        "View Classroom Discipline (Admin)",
                        "View Classroom Grades",
                        "View Classroom Lunch Count",
                        "View Classroom Roster",
                        "Maintain Discipline",
                        "View Discipline",
                        "Change Activity Dates",
                        "Maintain Classroom",
                        "Maintain Grade Book Averaging Method",
                        "Maintain Grade Book Categories",
                        "Maintain Standards Options",
                        "Maintain Student Averages",
                        "Reconcile GradeBook",
                        "View Classroom",
                        "Maintain Grading",
                        "View Grading",
                        "Maintain Lookup",
                        "View Lookup",
                        "Maintain Person",
                        "View Person",
                        "View Course",
                        "View Model",
                        "View Section",
                        "Maintain Locker",
                        "View Locker",
                        "View Staff",
                        "Maintain Student",
                        "Maintain Student Form",
                        "View Health Condition",
                        "View Medical",
                        "View Registration",
                        "View Special Education",
                        "View Special Instructions",
                        "View Student",
                        "View Student Commendations",
                        "View Student Custom",
                        "View Student Form",
                        "View Student Miscellaneous",
                        "Maintain Student Filter",
                        "View Student Filter"
                    }
                }
            };
        }


        public string GetUserEmailById(Guid id)
        {
            if (Context.UserId == id) 
                return Context.Login;

           throw new NotImplementedException();
        }

        public IList<User> GetAll(Guid districtId)
        {
            throw new NotImplementedException();
        }
    }
}