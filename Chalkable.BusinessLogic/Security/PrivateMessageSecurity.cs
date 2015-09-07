using Chalkable.BusinessLogic.Services;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Chlk;

namespace Chalkable.BusinessLogic.Security
{
    public  static class PrivateMessageSecurity
    {
        public static bool CanMarkMessage(PrivateMessage privateMessage, UserContext context)
        {
            return CanSendMessage(context) || privateMessage.ToPersonRef == context.PersonId;
        }
        public static bool CanDeleteMessage(PrivateMessage privateMessage, UserContext context)
        {
            return CanSendMessage(context) || privateMessage.FromPersonRef == context.PersonId ||
                   privateMessage.ToPersonRef == context.PersonId;
        }

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
