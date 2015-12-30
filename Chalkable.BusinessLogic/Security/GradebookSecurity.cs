using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.BusinessLogic.Security
{
    public class GradebookSecurity
    {
        public static bool CanReCalculateGradebook(UserContext context, bool isTeacherClass)
        {
            return (BaseSecurity.HasClaim(ClaimInfo.MAINTAIN_CLASSROOM, context) && isTeacherClass)
                   || BaseSecurity.HasClaim(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN, context);
        }
    }
}
