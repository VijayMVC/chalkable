using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ClaimInfo
    {
        public const string VIEW_LOOKUP = "View Lookup";
        public const string VIEW_CLASSROOM = "View Classroom";
        public const string VIEW_CLASSROOM_ADMIN = "View Classroom (Admin)";
        public const string MAINTAIN_CLASSROOM = "Maintain Classroom";
        public const string MAINTAIN_CLASSROOM_ADMIN = "Maintain Classroom (Admin)";
        public const string VIEW_HEALTH_CONDITION = "View Health Condition";
        public const string VIEW_MEDICAL = "View Medical";
        public const string CHALKABLE_ADMIN = "Administer Chalkable";
        public const string MAINTAIN_CHALKABLE_DISTRICT_SETTINGS = "Maintain Chalkable District Settings";
        public const string AWARD_LE_CREDITS_CLASSROOM = "Award LE Credits (Classroom)";
        public const string AWARD_LE_CREDITS = "Award LE Credits";
        public const string ASSESSMENT_ADMIN = "Assessment Admin";
        public const string VIEW_STUDENT = "View Student";
        public const string VIEW_CLASSROOM_STUDENTS = "View Classroom Students";

        public string Type { get; set; }
        public IEnumerable<string> Values { get; set; }

        public static IList<ClaimInfo> Create(IList<Claim> claims)
        {
            return claims.Select(claim => new ClaimInfo
                {
                    Type = claim.Type,
                    Values = claim.Values
                }).ToList();
        } 
        
    }

    public static class ClaimListHelper
    {
        public static bool HasPermissions(this IList<ClaimInfo> claimInfos, IList<string> claimsValues)
        {
            return claimInfos != null && claimInfos.Any(claim => claimsValues.All(value => claim.Values.Contains(value)));
        }

        public static bool HasPermission(this IList<ClaimInfo> claimInfos, string permission)
        {
            return HasPermissions(claimInfos, new List<string> { permission });
        }

        public static bool HasOneOfPermissions(this IList<ClaimInfo> claimInfos, IList<string> claimsValues)
        {
            return claimInfos != null && claimInfos.Any(claim => claimsValues.Any(value => claim.Values.Contains(value)));
        }
    }
}
