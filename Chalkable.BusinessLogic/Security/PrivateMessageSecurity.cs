using Chalkable.BusinessLogic.Services;
using Chalkable.Common.Exceptions;

namespace Chalkable.BusinessLogic.Security
{
    public  static class PrivateMessageSecurity
    {
        public static bool CanSendMessage(UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || !context.MessagingDisabled;
        }

        public static void EnsureMessgingPermission(UserContext context)
        {
            if (!CanSendMessage(context))
                throw new MessagingDisabledException();
        }
    }
}
