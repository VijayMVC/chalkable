using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class UserSecurity
    {
        public static bool CanCreate(UserContext context, Guid? districtId)
        {
            return BaseSecurity.IsSysAdmin(context) ||
                   (BaseSecurity.IsAdminEditor(context) && context.DistrictId == districtId);
        }

        public static bool CanModify(UserContext context, User user)
        {
            var districtId = user.DistrictRef;
            return CanCreate(context, districtId) || context.UserId == user.Id;
        }
    }
}
