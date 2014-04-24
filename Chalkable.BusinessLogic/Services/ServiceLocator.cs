using System;
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
        
        public static IServiceLocatorSchool CreateSchoolLocator(SchoolUser schoolUser)
        {
           var user = schoolUser.User;
           var school = schoolUser.School;
           var role = CoreRoles.GetById(schoolUser.Role);
           //TODO: how to get SIS token for OAuth
           var context = new UserContext(user, role, user.District, school, null);
           //var context = new UserContext(schoolUser.UserRef, schoolUser.School.DistrictRef, schoolUser.SchoolRef, user.Login, 
           //                               user.District.TimeZone, user.District.ServerUrl, schoolUser.School.LocalId, role, null, user.LocalId, null, null);
           return CreateSchoolLocator(context);
        }

        public static IServiceLocatorSchool CreateSchoolLocator(UserContext context)
        {
            var masterLocator = new ServiceLocatorMaster(context);
            return new ServiceLocatorSchool(masterLocator);
        }
    }
}