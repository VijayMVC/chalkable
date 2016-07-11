using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Common;
using Chalkable.Web.Models;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.SchoolsViewData;
using Chalkable.Web.Models.Settings;

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

            var allApps = MasterLocator.ApplicationService.GetApplications(live: true)
                .Where(x => x.HasDistrictAdminSettings)
                .Select(BaseApplicationViewData.Create)
                .ToList();

            if (ApplicationSecurity.HasAssessmentEnabled(Context) && Context.Claims.HasPermission(ClaimInfo.ASSESSMENT_ADMIN))
            {
                var assessement = MasterLocator.ApplicationService.GetAssessmentApplication();
                if (assessement != null && assessement.HasDistrictAdminSettings && !allApps.Exists(x => x.Id == assessement.Id))
                    allApps.Add(BaseApplicationViewData.Create(assessement));               
            }
            else
            {
                var assessmentId = SchoolLocator.ServiceLocatorMaster.ApplicationService.GetAssessmentId();
                allApps = allApps.Where(x => x.Id != assessmentId).ToList();
            }
            
            return Json(DistrictAdminSettingsViewData.Create(messagingSettings, allApps));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult PanoramaSettings()
        {
            if (!Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CHALKABLE_DISTRICT_SETTINGS))
                throw new ChalkableSecurityException("You are not allowed to change panorama settings");

            var courseTypes = SchoolLocator.CourseTypeService.GetList(true);
            var settings = SchoolLocator.PanoramaSettingsService.Get<AdminPanoramaSettings>(null);
            return Json(AdminPanoramaSettingsViewData.Create(settings, courseTypes));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult SavePanoramaSettings(AdminPanoramaSettings settings)
        {
            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to change panorama settings");

            settings = settings ?? SchoolLocator.PanoramaSettingsService.GetDefaultSettings<AdminPanoramaSettings>();
            SchoolLocator.PanoramaSettingsService.Save(settings, null);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult StandardizedTests()
        {
            var tests = SchoolLocator.StandardizedTestService.GetListOfStandardizedTestDetails();
            return Json(tests.Select(x => StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)));
        }
    }
}