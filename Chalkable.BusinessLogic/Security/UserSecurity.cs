using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class UserSecurity
    {
        public static bool CanCreate(UserContext context, Guid schoolId)
        {
            return BaseSecurity.IsSysAdmin(context) ||
                   (BaseSecurity.IsAdminEditor(context) && context.SchoolId == schoolId);
        }

        public static bool CanModify(UserContext context, User user)
        {
            var schoolId = user.SchoolUsers.First().SchoolRef;
            return CanCreate(context, schoolId) || context.UserId == user.Id;
        }
    }
}
