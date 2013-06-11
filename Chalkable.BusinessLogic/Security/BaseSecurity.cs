using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.BusinessLogic.Security
{
    public class BaseSecurity
    {
        public static bool IsSysAdmin(UserContext context)
        {
            return context.Role ==  CoreRoles.SUPER_ADMIN_ROLE;
        }
        public static bool IsAdminGrader(UserContext context)
        {
            return context.Role ==  CoreRoles.ADMIN_GRADE_ROLE;
        }
        public static bool IsAdminEditor(UserContext context)
        {
            return IsAdminGrader(context) || context.Role == CoreRoles.ADMIN_EDIT_ROLE;
        }
        public static bool IsAdminViewer(UserContext context)
        {
            return IsAdminEditor(context) || context.Role == CoreRoles.ADMIN_VIEW_ROLE;
        }
    }
}
