using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public  static class PrivateMessageSecurity
    {
        public static bool CanMarkMessage(PrivateMessage privateMessage, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || privateMessage.ToPersonRef == context.UserId;
        }
        public static bool CanDeleteMessage(PrivateMessage privateMessage, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || privateMessage.FromPersonRef == context.UserId ||
                   privateMessage.ToPersonRef == context.UserId;
        }
    }
}
