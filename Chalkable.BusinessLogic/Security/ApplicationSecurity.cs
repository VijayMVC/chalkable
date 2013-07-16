using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public class ApplicationSecurity
    {
        public static bool CanAddToAnnouncement(Application application, Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || (announcement.PersonRef == context.UserId && application.CanAttach);
        }
    }
}