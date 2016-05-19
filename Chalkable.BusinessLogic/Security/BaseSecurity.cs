using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
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

        public static bool IsSysAdminOrDeveloper(UserContext context)
        {
            return IsSysAdmin(context) || context.Role == CoreRoles.DEVELOPER_ROLE;
        }

        public static void EnsureSysAdmin(UserContext context)
        {
            if (!IsSysAdmin(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsDistrictAdmin(UserContext context)
        {
            return IsSysAdmin(context) || context.Role == CoreRoles.DISTRICT_ADMIN_ROLE;
        }

        public static void EnsureSysAdminOrDistrictRegistrator(UserContext context)
        {
            if (!IsSysAdmin(context) &&  !IsDistrictRegistrator(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsDistrictRegistrator(UserContext context)
        {
            return context.Role == CoreRoles.DISTRICT_REGISTRATOR_ROLE;
        }

        public static void EnsureDistrictAdmin(UserContext context)
        {
            if (!IsDistrictAdmin(context))
                throw new ChalkableSecurityException();
        }
        
        public static bool IsDistrictOrTeacher(UserContext context)
        {
            return IsDistrictAdmin(context) || IsTeacher(context);
        }
        
        public static bool IsTeacher(UserContext context)
        {
            return context.Role == CoreRoles.TEACHER_ROLE;
        }

        public static bool IsStudent(UserContext context)
        {
            return context.Role == CoreRoles.STUDENT_ROLE;
        }

        public static void EnsureTeacher(UserContext context)
        {
            if(!IsTeacher(context))
                throw new ChalkableSecurityException();
        }

        public static void EnsureAdminOrTeacher(UserContext context)
        {
            if (!IsDistrictOrTeacher(context))
                throw new ChalkableSecurityException();
        }

        public static bool IsDistrictAdminOrClassTeacher(Class c, UserContext context)
        {
            return IsDistrictAdmin(context) || (context.Role == CoreRoles.TEACHER_ROLE && context.PersonId == c.PrimaryTeacherRef);
        }

        public static bool HasChalkableRole(UserContext context)
        {
            return IsDistrictOrTeacher(context) || context.Role == CoreRoles.STUDENT_ROLE
                   || context.Role == CoreRoles.PARENT_ROLE || context.Role == CoreRoles.DEVELOPER_ROLE
                   || context.Role == CoreRoles.CHECKIN_ROLE;
        }

        public static bool IsDistrictAdminOrCurrentPerson(int personId, UserContext context)
        {
            return IsDistrictAdmin(context) || context.PersonId == personId;
        }

        public static bool IsDistrictAdminTeacherOrExactStudent(User user, UserContext context)
        {
            return IsDistrictOrTeacher(context) || user.Id == context.UserId;
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

        public static bool HasClaim(string claim, UserContext context)
        {
            return context.Claims.HasPermission(claim);
        }

        public static bool IsAppTester(UserContext context)
        {
            return context.Role == CoreRoles.APP_TESTER_ROLE;
        }
        
        public static void EnsureStudyCenterEnabled(UserContext context)
        {
            if(!context.SCEnabled)
                throw new StudyCenterDisabledException();
        }
    }
}
