using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Security
{
    public class ApplicationSecurity
    {
        public static bool CanAddToAnnouncement(Application application, Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || (announcement.IsOwner && application.CanAttach);
        }

        public static bool CanEditApplication(UserContext context, Application application)
        {
            return BaseSecurity.IsSysAdmin(context) ||
                  (context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id && context.UserId == application.DeveloperRef);
        }

        public static bool CanUploadApplication(UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id;
        }

        public static bool CanUninstall(UserContext context, ApplicationInstall applicationInstall)
        {
            return context.PersonId == applicationInstall.OwnerRef;
        }
    }
}