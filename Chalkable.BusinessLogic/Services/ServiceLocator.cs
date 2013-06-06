using System;
using System.Configuration;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services
{
    public class ServiceLocator
    {
        public ServiceContext Context { get; private set; }

        public ServiceLocator(ServiceContext context)
        {
            Context = context;
        }
    }

    public static class ServiceLocatorFactory
    {
        public static ServiceLocatorMaster CreateMasterSysAdmin()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ChalkableMaster"].ConnectionString;
            var context = new ServiceContext(connectionString, Guid.Empty, "Virtual system admin", CoreRoles.SUPER_ADMIN_ROLE);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }

        /*public static ServiceLocatorMaster CreateMaster(User user)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ChalkableMaster"].ConnectionString;
            CoreRole role;
            if (user.IsSysAdmin)
                role = CoreRoles.SUPER_ADMIN_ROLE;
            else if (user.IsDeveloper)
                role = CoreRoles.DEVELOPER_ROLE;
            else if (user.SchoolUsers != null && user.SchoolUsers.Count > 0)
                role = CoreRoles.GetById(user.SchoolUsers[0].Role);
            else
                throw new Exception("User does not belong to any role");
            var context = new ServiceContext(connectionString, user.Id, user.Login, role);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }*/
    }
}