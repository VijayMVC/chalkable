using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FeedController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult List(int? start, int? count, bool? complete, int? classId)
        {
            var graded = !BaseSecurity.IsDistrictAdmin(SchoolLocator.Context);
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, true, graded));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictAdminFeed(IntList gradeLevelIds, bool? complete, int? start, int? count)
        {
            var announcements = SchoolLocator.AnnouncementService.GetAdminAnnouncements(complete, gradeLevelIds, null, null, start ?? 0, count ?? 10, true);
            return Json(GetAnnouncementForFeedList(SchoolLocator, announcements));
        }

        public static IList<AnnouncementViewData> GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, bool ownerOnly =false, bool? graded = null)
        {
            start = start ?? 0;
            count = count ?? (DemoUserService.IsDemoUser(schoolL.Context) ? int.MaxValue : 10);
            var list = schoolL.AnnouncementService.GetAnnouncements(complete, start.Value, count.Value, classId, null, ownerOnly, graded);
            return GetAnnouncementForFeedList(schoolL, list);
        }

        private static IList<AnnouncementViewData> GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, IList<AnnouncementComplex> announcements)
        {
            if (DemoUserService.IsDemoUser(schoolL.Context))
                announcements = announcements.Where(x => x.State == AnnouncementState.Created).Take(10).ToList();
            var annsIdsWithApp = announcements.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return AnnouncementViewData.Create(announcements, annApps, apps);
        }
    }
}