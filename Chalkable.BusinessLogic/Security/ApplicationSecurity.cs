using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Security
{
    public class ApplicationSecurity
    {
        public static bool CanAddToAnnouncement(Application application, Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || (announcement.IsOwner && application.CanAttach) || BaseSecurity.IsDistrictAdmin(context);
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

        public static bool HasAccessToBannedApps(UserContext context)
        {
            return BaseSecurity.IsDistrictAdmin(context) || context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id;
        }

        public static bool HasAssessmentEnabled(UserContext context)
        {
            return context.AssessmentEnabled || BaseSecurity.IsSysAdmin(context) || context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id || context.Role.Id == CoreRoles.APP_TESTER_ROLE.Id;
        }

        public static bool HasStudyCenterAccess(UserContext context)
        {
            return context.SCEnabled || BaseSecurity.IsSysAdmin(context) || context.Role.Id == CoreRoles.DEVELOPER_ROLE.Id;
        }
    }
}