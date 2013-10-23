using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services
{
    public interface IServiceLocator
    {
        IStorageBlobService StorageBlobService { get; }
    }

    public class ServiceLocator : IServiceLocator
    {
        public UserContext Context { get; private set; }

        private IStorageBlobService storageBlobService;
        public ServiceLocator(UserContext context)
        {
            Context = context;
            StorageBlobService = new StorageBlobService();
        }

        public IStorageBlobService StorageBlobService
        {
            get { return storageBlobService; }
            protected set { storageBlobService = value; }
        }
    }

    public static class ServiceLocatorFactory
    {
        public static IServiceLocatorMaster CreateMasterSysAdmin()
        {
            var context = new UserContext(Guid.Empty, null, "Virtual system admin", null, null, null, CoreRoles.SUPER_ADMIN_ROLE, null);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }
        
        public static IServiceLocatorSchool CreateSchoolLocator(SchoolUser schoolUser)
        {
           var user = schoolUser.User;
           var school = schoolUser.School;
           var role = CoreRoles.GetById(schoolUser.Role);
           var context = new UserContext(schoolUser.UserRef, schoolUser.SchoolRef, user.Login, school.Name,
                                          school.TimeZone, school.ServerUrl, role, null);
           return CreateSchoolLocator(context);
        }

        public static IServiceLocatorSchool CreateSchoolLocator(UserContext context)
        {
            var masterLocator = new ServiceLocatorMaster(context);
            return new ServiceLocatorSchool(masterLocator);
        }
    }
}