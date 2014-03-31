using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Caching;
using Chalkable.BusinessLogic.Services.DemoSchool;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

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
            var admin = new User {Id = Guid.Empty, Login = "Virtual system admin"};
            var context = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null);
            //var context = new UserContext(Guid.Empty, null, null, "Virtual system admin", null, null, null, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }

        private static bool IsDemoUser(UserContext context)
        {
            var userLogin = context.Login.Replace(context.DistrictId.ToString(), "");
            var logins = new string[]
            {
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_EDIT).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_GRADE).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_ADMIN_VIEW).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_TEACHER).Value,
                PreferenceService.Get(Preference.DEMO_SCHOOL_STUDENT).Value
            };
                
            return logins.Any(login => String.Equals(userLogin, login, StringComparison.CurrentCultureIgnoreCase));
        }
        
        public static IServiceLocatorSchool CreateSchoolLocator(SchoolUser schoolUser)
        {
            return CreateSchoolLocator(CreateUserContext(schoolUser));
        }

        private static UserContext CreateUserContext(SchoolUser schoolUser)
        {
            var user = schoolUser.User;
            var school = schoolUser.School;
            var role = CoreRoles.GetById(schoolUser.Role);
            //TODO: how to get SIS token for OAuth
            return new UserContext(user, role, user.District, school, null);
            //var context = new UserContext(schoolUser.UserRef, schoolUser.School.DistrictRef, schoolUser.SchoolRef, user.Login, 
            //                               user.District.TimeZone, user.District.ServerUrl, schoolUser.School.LocalId, role, null, user.LocalId, null, null);
        }

        public static IServiceLocatorSchool CreateSchoolLocator(UserContext context)
        {
            IServiceLocatorSchool locator;
            
            if (context != null && IsDemoUser(context))
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
            //if (!MemoryCache.Default.Contains(context.UserId.ToString()))
            //{
               // MemoryCache.Default[context.UserId.ToString()] = new DemoStorage(context);
            //}
            //var storage = (DemoStorage)MemoryCache.Default[context.UserId.ToString()];

            var storage = new DemoStorage(context);
            var masterLocator = new DemoServiceLocatorMaster(context, storage);
            return new DemoServiceLocatorSchool(masterLocator, storage);
        }
    }
}