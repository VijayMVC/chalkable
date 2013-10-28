using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
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
            return IsAdminEditor(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.UserLocalId == c.TeacherRef);
        }

        public static bool IsAdminViewerOrClassTeacher(Class c, UserContext context)
        {
            return IsAdminViewer(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.UserLocalId == c.TeacherRef);
        }

        public static bool HasChalkableRole(UserContext context)
        {
            return IsAdminOrTeacher(context) || context.Role == CoreRoles.STUDENT_ROLE
                   || context.Role == CoreRoles.PARENT_ROLE || context.Role == CoreRoles.DEVELOPER_ROLE
                   || context.Role == CoreRoles.CHECKIN_ROLE;
        }

        public static bool IsAdminEditorOrCurrentPerson(int personId, UserContext context)
        {
            return IsAdminEditor(context) || context.UserLocalId == personId;
        }

        public static bool IsAdminTeacherOrExactStudent(User user, UserContext context)
        {
            return IsAdminOrTeacher(context) || user.Id == context.UserId;
        }
    }
}
