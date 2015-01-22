using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class ClaimInfo
    {
        public const string VIEW_LOOKUP = "View Lookup";
        public const string VIEW_CLASSROOM = "View Classroom";
        public const string VIEW_CLASSROOM_ADMIN = "View Classroom (Admin)";

        public const string VIEW_HEALTH_CONDITION = "View Health Condition";
        public const string VIEW_MEDICAL = "View Medical";

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

        public static bool HasPermission(IList<ClaimInfo> claimInfos, IList<string> claimsValues)
        {
            return claimInfos.Any(claim => claimsValues.All(value => claim.Values.Contains(value)));
        }
    }
}
