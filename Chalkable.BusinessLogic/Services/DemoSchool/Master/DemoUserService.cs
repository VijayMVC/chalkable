using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;

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

            var schoolUsers = new List<SchoolUser>();

            var school = new Chalkable.Data.Master.Model.School
            {
                DistrictRef = Guid.Parse(prefix),
                Id = Guid.NewGuid(),
                LocalId = 1,
                Name = "SMITH"
            };

            var userRef = Guid.NewGuid();
            schoolUsers.Add(new SchoolUser
            {
                Id = Guid.NewGuid(),
                Role = CoreRoles.GetByName(roleName).Id,
                SchoolRef = school.Id,
                UserRef = userRef
            });

            var district = new District
            {
                DbName = "DemoDB",
                DemoPrefix = prefix,
                Id = Guid.Parse(prefix),
                Name = "Demo District",
                TimeZone = "UTC"
            };


            //set local id for current user

            return new User
            {
                ConfirmationKey = null,
                DistrictRef = Guid.Parse(prefix),
                Id = userRef,
                LocalId = 1195,
                IsDeveloper = false,
                IsSysAdmin = false,
                Login = demoUserName,
                SchoolUsers = schoolUsers,
                District = district
            };
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
            using (var uow = Update())
            {
                var schoolUserDa = new SchoolUserDataAccess(uow);
                schoolUserDa.Insert(schoolUsers);
                uow.Commit();
            }
        }

        public void ChangePassword(string login, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void CreateSchoolUsers(IList<User> users)
        {
            throw new NotImplementedException();
            using (var uow = Update())
            {
                var userDa = new UserDataAccess(uow);
                userDa.Insert(users);
                uow.Commit();
            }
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