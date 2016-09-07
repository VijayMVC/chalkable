using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonClaimViewData
    {
        public string Type { get; set; }
        public IList<string> Values { get; set; }

        public static IList<PersonClaimViewData> Create(IList<ClaimInfo> claimInfos)
        {
            return claimInfos.Select(x => new PersonClaimViewData
            {
                Type = x.Type,
                Values = x.Values.ToList()
            }).ToList();
        }
    }
}