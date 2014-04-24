using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Master
{

    public class DemoUserService : DemoMasterServiceBase, IUserService
    {
        public DemoUserService(IServiceLocatorMaster serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public string PasswordMd5(string password)
        {
            throw new NotImplementedException();
        }

        public static User GetDemoUser(string roleName, string prefix)
        {
            var demoUserName = BuildDemoUserName(roleName, prefix);
            roleName = roleName.ToLowerInvariant();


            var schoolUsers = new List<SchoolUser>();

            var school = DemoMasterSchoolStorage.CreateMasterSchool(Guid.Parse(prefix));

            var userRef = Guid.NewGuid();


            var localIds = new Dictionary<string, int>
            {
                {CoreRoles.TEACHER_ROLE.LoweredName, 1195},
                {CoreRoles.STUDENT_ROLE.LoweredName, 1196},
                {CoreRoles.ADMIN_GRADE_ROLE.LoweredName, 1197},
                {CoreRoles.ADMIN_EDIT_ROLE.LoweredName, 1198},
                {CoreRoles.ADMIN_VIEW_ROLE.LoweredName, 1199},
                
            };

            var district = DemoDistrictStorage.CreateDemoDistrict(Guid.Parse(prefix));

            var user = new User
            {
                ConfirmationKey = null,
                DistrictRef = Guid.Parse(prefix),
                Id = userRef,
                LocalId = localIds[roleName],
                IsDeveloper = false,
                IsSysAdmin = false,
                Login = demoUserName,
     
                District = district,
                              
            };

            schoolUsers.Add(new SchoolUser
            {
                Id = Guid.NewGuid(),
                Role = CoreRoles.GetByName(roleName).Id,
                SchoolRef = school.Id,
                UserRef = userRef,
                School = school,
                User = user
            });

            user.SchoolUsers = schoolUsers;

            return user;
        }


        public UserContext Login(string login, string password)
        {
            throw new NotImplementedException();
        }
        public UserContext Login(string confirmationKey)
        {
            throw new NotImplementedException();
        }

        public UserContext LoginToDemo(string roleName, string demoPrefix)
        {
            throw new NotImplementedException();
        }

        public UserContext ReLogin(Guid id)
        {
            throw new NotImplementedException();
        }


        public UserContext SisLogIn(Guid sisDistrictId, string token, DateTime tokenExpiresTime)
        {
            throw new NotImplementedException();
        }

        private static string BuildDemoUserName(string roleName, string prefix)
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

        public IList<User> GetByDistrict(Guid districtId)
        {
            throw new NotImplementedException();
        }

        public User CreateSysAdmin(string login, string password)
        {
            throw new NotImplementedException();
        }

        public void AssignUserToSchool(IList<SchoolUser> schoolUsers)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string login, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void CreateSchoolUsers(IList<User> users)
        {
            throw new NotImplementedException();
        }


        public User CreateDeveloperUser(string login, string password)
        {
            throw new NotImplementedException();
        }

        public User CreateSchoolUser(string login, string password, Guid? districtId, int? localId, string sisUserName)
        {
            throw new NotImplementedException();
        }

        public void ChangeUserLogin(Guid id, string login)
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

      
    }
}