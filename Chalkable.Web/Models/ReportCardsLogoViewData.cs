using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ReportCardsLogoViewData
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string LogoAddress { get; set; }
        public bool IsDistrictLogo { get; set; }

        public static IList<ReportCardsLogoViewData> Create(IList<ReportCardsLogo> reportsLogos, IList<School> schools)
        {
            var res = new List<ReportCardsLogoViewData>();
            foreach (var logo in reportsLogos)
            {
                var school = schools.FirstOrDefault(x => x.Id == logo.SchoolRef);
                if(logo.SchoolRef.HasValue && school == null) continue;
                res.Add(new ReportCardsLogoViewData
                {
                    Id = logo.Id,
                    SchoolId = logo.SchoolRef,
                    SchoolName = school?.Name,
                    LogoAddress = logo.LogoAddress,
                    IsDistrictLogo = !logo.SchoolRef.HasValue
                });
            }
            return res;
        } 
    }
}