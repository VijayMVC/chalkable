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
    public class BaseSecurity
    {
        public static bool IsSysAdmin(UserContext context)
        {
            return context.Role ==  CoreRoles.SUPER_ADMIN_ROLE;
        }
        public static bool IsAdminGrader(UserContext context)
        {
            return IsSysAdmin(context) || context.Role ==  CoreRoles.ADMIN_GRADE_ROLE;
        }
        public static bool IsAdminEditor(UserContext context)
        {
            return IsAdminGrader(context) || context.Role == CoreRoles.ADMIN_EDIT_ROLE;
        }
        public static bool IsAdminViewer(UserContext context)
        {
            return IsAdminEditor(context) || context.Role == CoreRoles.ADMIN_VIEW_ROLE;
        }

        public static bool IsAdminOrTeacher(UserContext context)
        {
            return IsAdminViewer(context) || context.Role == CoreRoles.TEACHER_ROLE;
        }

        public static bool IsAdminEditorOrClassTeacher(Class c, UserContext context)
        {
            return IsAdminEditor(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.UserId == c.TeacherRef);
        }

        public static bool HasChalkableRole(UserContext context)
        {
            return IsAdminOrTeacher(context) || context.Role == CoreRoles.STUDENT_ROLE
                   || context.Role == CoreRoles.PARENT_ROLE || context.Role == CoreRoles.DEVELOPER_ROLE
                   || context.Role == CoreRoles.CHECKIN_ROLE;
        }

        public static bool IsAdminEditorOrCurrentPerson(Guid personId, UserContext context)
        {
            return IsAdminEditor(context) || context.UserId == personId;
        }
    }
}
