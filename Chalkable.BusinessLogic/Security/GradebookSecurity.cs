using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.BusinessLogic.Security
{
    public class GradebookSecurity
    {
        public static bool CanReCalculateGradebook(UserContext context)
        {
            return BaseSecurity.HasClaim(ClaimInfo.VIEW_CLASSROOM, context)
                   || BaseSecurity.HasClaim(ClaimInfo.VIEW_CLASSROOM_ADMIN, context);
        }
    }
}
