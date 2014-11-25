using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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

        public static void EnsureSysAdmin(UserContext context)
        {
            if (!IsSysAdmin(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsDistrict(UserContext context)
        {
            return IsSysAdmin(context) || context.Role == CoreRoles.DISTRICT_ROLE;
        }

        public static void EnsureDistrict(UserContext context)
        {
            if (!IsDistrict(context))
                throw new ChalkableSecurityException();
        }
        
        public static bool IsAdminGrader(UserContext context)
        {
            return IsDistrict(context) || context.Role ==  CoreRoles.ADMIN_GRADE_ROLE;
        }

        public static void EnsureAdminGrader(UserContext context)
        {
            if (!IsAdminGrader(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsAdminEditor(UserContext context)
        {
            return IsAdminGrader(context) || context.Role == CoreRoles.ADMIN_EDIT_ROLE;
        }

        public static void EnsureAdminEditor(UserContext context)
        {
            if (!IsAdminEditor(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsAdminViewer(UserContext context)
        {
            return IsAdminEditor(context) || context.Role == CoreRoles.ADMIN_VIEW_ROLE;
        }

        public static void EnsureAdminViewer(UserContext context)
        {
            if (!IsAdminViewer(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsAdminOrTeacher(UserContext context)
        {
            return IsAdminViewer(context) || context.Role == CoreRoles.TEACHER_ROLE;
        }

        public static void EnsureAdminOrTeacher(UserContext context)
        {
            if (!IsAdminOrTeacher(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsAdminEditorOrClassTeacher(Class c, UserContext context)
        {
            return IsAdminEditor(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.PersonId == c.PrimaryTeacherRef);
        }

        public static bool IsAdminViewerOrClassTeacher(Class c, UserContext context)
        {
            return IsAdminViewer(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.PersonId == c.PrimaryTeacherRef);
        }

        public static bool HasChalkableRole(UserContext context)
        {
            return IsAdminOrTeacher(context) || context.Role == CoreRoles.STUDENT_ROLE
                   || context.Role == CoreRoles.PARENT_ROLE || context.Role == CoreRoles.DEVELOPER_ROLE
                   || context.Role == CoreRoles.CHECKIN_ROLE;
        }

        public static bool IsAdminEditorOrCurrentPerson(int personId, UserContext context)
        {
            return IsAdminEditor(context) || context.PersonId == personId;
        }

        public static bool IsAdminTeacherOrExactStudent(User user, UserContext context)
        {
            return IsAdminOrTeacher(context) || user.Id == context.UserId;
        }

        public static bool IsSysAdminOrCurrentUser(Guid userId, UserContext context)
        {
            return IsSysAdmin(context) || context.UserId == userId;
        }

        public static void EnsureSysAdminOrCurrentUser(Guid userId, UserContext context)
        {
            if (!IsSysAdminOrCurrentUser(userId, context))
                throw new ChalkableSecurityException();
        }
    }
}
