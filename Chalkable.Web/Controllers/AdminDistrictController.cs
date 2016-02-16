using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.SchoolsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class AdminDistrictController : ChalkableController
    {
        // GET: AdminDistrict
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictSummary()
        {
            Trace.Assert(Context.DistrictId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
            var schools = SchoolLocator.SchoolService.GetShortSchoolSummariesInfo(0, int.MaxValue, null, null);
            var schoolsCount = schools.Count;
            var studentsCount = schools.Sum(x => x.SchoolDetails.StudentsCount);
            return Json(ShortDistrictSummaryViewData.Create(district, studentsCount, schoolsCount));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Schools(string filter, int? start, int? count, int? sortType)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var schools = SchoolLocator.SchoolService.GetShortSchoolSummariesInfo(start, count, filter, (SchoolSortType?) sortType);

            return Json(schools.Select(LocalSchoolSummaryViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Settings()
        {
            Trace.Assert(Context.DistrictId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);

            var messagingSettings = MessagingSettingsViewData.Create(MasterLocator.SchoolService.GetDistrictMessaginSettings(Context.DistrictId.Value));
            PrepareJsonData(messagingSettings, ViewConstants.MESSAGING_SETTINGS);
            
            var installedApps = AppMarketController.GetListInstalledApps(SchoolLocator, MasterLocator, Context.PersonId.Value, null, 0, int.MaxValue, null).ToList();
            installedApps = installedApps.Where(x => x.HasDistrictAdminSettings).ToList();

            var assessement = MasterLocator.ApplicationService.GetAssessmentApplication();
            if (assessement.HasDistrictAdminSettings && !installedApps.Exists(x => x.Id == assessement.Id))
            {
                var assAppInstallations = SchoolLocator.AppMarketService.ListInstalledAppInstalls(Context.PersonId.Value);
                var hasMyApp = MasterLocator.ApplicationService.HasMyApps(assessement);
                installedApps.Add(InstalledApplicationViewData.Create(assAppInstallations, Context.PersonId.Value, assessement, hasMyApp));
            }

            return Json(DistrictAdminSettingsViewData.Create(messagingSettings, installedApps));
        }
    }
}