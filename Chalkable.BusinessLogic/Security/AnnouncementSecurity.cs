using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class AnnouncementSecurity
    {
        public static bool CanModifyAnnouncement(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsAdminEditor(context) || context.UserId == announcement.PersonRef;
        }
    }
}
