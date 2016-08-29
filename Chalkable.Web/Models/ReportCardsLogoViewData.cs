using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ReportCardsLogoViewData
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string LogoAddress { get; set; }
        public bool IsDistrictLogo { get; set; }

        public static IList<ReportCardsLogoViewData> Create(IList<ReportCardsLogo> reportsLogos)
        {
            return reportsLogos.Select(x => new ReportCardsLogoViewData
            {
                Id = x.Id,
                SchoolId = x.SchoolRef,
                LogoAddress = x.LogoAddress,
                IsDistrictLogo = !x.SchoolRef.HasValue
            }).ToList();
        } 
    }
}