using System.Collections.Generic;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.SchoolsViewData;

namespace Chalkable.Web.Models
{
    public class ReportCardsSettingViewData
    {
        public IList<ReportCardsLogoViewData> ListOfReportCardsLogo { get; set; }
        public IList<LocalSchoolViewData> Schools { get; set; }

        public static ReportCardsSettingViewData Create(IList<ReportCardsLogo> listLogo, IList<School> allSchools)
        {
            return new ReportCardsSettingViewData
            {
                Schools = LocalSchoolViewData.Create(allSchools),
                ListOfReportCardsLogo = ReportCardsLogoViewData.Create(listLogo, allSchools)
            };
        }
    }
}