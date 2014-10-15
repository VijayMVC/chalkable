using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class NotificationSecurity
    {
        public static bool CanCreateAnnouncementNotification(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcement.IsOwner;
        }

        public static bool CanCreateSimpleNotification(Guid personId, UserContext context)
        {
            return BaseSecurity.IsAdminOrTeacher(context);
        }

        public static bool CanModify(Notification notification, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || notification.PersonRef == context.PersonId;
        }
    }
}
