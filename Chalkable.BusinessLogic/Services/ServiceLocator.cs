using System;
using System.Web;
using System.Web.Caching;
using Chalkable.BusinessLogic.Services.DemoSchool;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using CacheItemPriority = System.Web.Caching.CacheItemPriority;

namespace Chalkable.BusinessLogic.Services
{
    public interface IServiceLocator
    {
        IStorageBlobService StorageBlobService { get; }
        ICrocodocService CrocodocService { get; }
    }

    public class ServiceLocator : IServiceLocator
    {
        public UserContext Context { get; private set; }

        private IStorageBlobService storageBlobService;
        public ServiceLocator(UserContext context)
        {
            Context = context;
            StorageBlobService = new StorageBlobService();
            CrocodocService = new CrocodocService();
        }

        public IStorageBlobService StorageBlobService
        {
            get { return storageBlobService; }
            protected set { storageBlobService = value; }
        }


        public ICrocodocService CrocodocService { get; protected set; }
    }

    public static class ServiceLocatorFactory
    {
        public static IServiceLocatorMaster CreateMasterSysAdmin()
        {
            var admin = new User {Id = Guid.Empty, Login = "Virtual system admin", LoginInfo = new UserLoginInfo()};
            var context = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }

        public static IServiceLocatorSchool CreateSchoolLocator(SchoolUser schoolUser, Data.School.Model.SchoolYear schoolYear = null)
        {
            var context = CreateUserContext(schoolUser, schoolYear);
            return CreateSchoolLocator(context);
        }

        private static UserContext CreateUserContext(SchoolUser schoolUser, Data.School.Model.SchoolYear schoolYear = null)
        {
            int roleId;

            int personId = schoolUser.User.IsDemoUser
                ? DemoPersonStorage.GetPersonDataForLogin(schoolUser.User, out roleId)
                : PersonDataAccess.GetPersonDataForLogin(schoolUser.User.District.ServerUrl,
                                                                  schoolUser.DistrictRef, schoolUser.UserRef, out roleId);
            var user = schoolUser.User;
            var school = schoolUser.School;
            var role = CoreRoles.GetById(roleId);

            Guid? developerId = null;

            if (schoolUser.User.IsDemoUser)
            {
                var developer = CreateMasterSysAdmin().UserService.GetById(user.District.Id);
                if (developer != null)
                    developerId = developer.Id;
            }
            return new UserContext(user, role, user.District, school, developerId, personId, schoolYear);
        }

        public static IServiceLocatorSchool CreateSchoolLocator(UserContext context)
        {
            IServiceLocatorSchool locator;
            
            if (context != null && DemoUserService.IsDemoUser(context))
            {
                locator = CreateDemoSchoolLocator(context);
            }
            else
            {
                var masterLocator = new ServiceLocatorMaster(context);
                locator = new ServiceLocatorSchool(masterLocator);
            }

            return locator;
        }

        private static IServiceLocatorSchool CreateDemoSchoolLocator(UserContext context)
        {
            //district ref not user id
            if (HttpRuntime.Cache[context.DistrictId.ToString()] == null)
            {
                HttpRuntime.Cache.Add(context.DistrictId.ToString(), new DemoStorage(context), null,
                    DateTime.Now.AddHours(3), Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
            var storage = (DemoStorage)HttpRuntime.Cache[context.DistrictId.ToString()];
            storage.UpdateContext(context);
            var masterLocator = new DemoServiceLocatorMaster(context, storage);
            return new DemoServiceLocatorSchool(masterLocator, storage);
        }
    }
}