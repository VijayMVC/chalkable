using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Security
{
    public class AttendanceSecurity
    {
        public static bool CanSetDailyAttendance(UserContext context)
        {
            return BaseSecurity.IsAdminViewer(context) || context.Role == CoreRoles.CHECKIN_ROLE;
        }
    }
}
