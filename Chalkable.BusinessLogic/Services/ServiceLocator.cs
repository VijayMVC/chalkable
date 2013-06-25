using System;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Services
{
    public class ServiceLocator
    {
        public UserContext Context { get; private set; }

        public ServiceLocator(UserContext context)
        {
            Context = context;
        }
    }

    public static class ServiceLocatorFactory
    {
        public static IServiceLocatorMaster CreateMasterSysAdmin()
        {
            var context = new UserContext(Guid.Empty, null, "Virtual system admin", null, null, null, CoreRoles.SUPER_ADMIN_ROLE);
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
                role = CoreRoles.GetMarkingPeriodById(user.SchoolUsers[0].Role);
            else
                throw new Exception("User does not belong to any role");
            var context = new UserContext(connectionString, user.Id, user.Login, role);
            var serviceLocator = new ServiceLocatorMaster(context);
            return serviceLocator;
        }*/
    }
}