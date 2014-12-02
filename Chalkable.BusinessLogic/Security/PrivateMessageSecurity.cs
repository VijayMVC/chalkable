using Chalkable.BusinessLogic.Services;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public  static class PrivateMessageSecurity
    {
        public static bool CanMarkMessage(PrivateMessage privateMessage, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || privateMessage.ToPersonRef == context.PersonId;
        }
        public static bool CanDeleteMessage(PrivateMessage privateMessage, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || privateMessage.FromPersonRef == context.PersonId ||
                   privateMessage.ToPersonRef == context.PersonId;
        }
    }
}
