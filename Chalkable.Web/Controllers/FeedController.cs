using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FeedController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_FEED_LIST, true, CallType.Get, new[] { AppPermissionType.Announcement })]
        public ActionResult List(int? start, int? count, bool? complete, int? classId)
        {
            //start = start ?? 0;
            //count = count ?? 10;
            //var list = SchoolLocator.AnnouncementService.GetAnnouncements(complete, start.Value, count.Value, 
            //    classId, null, BaseSecurity.IsAdminViewer(SchoolLocator.Context));

            //var annsIdsWithApp = list.Where(x => x.ApplicationCount == 1).Select(x=>x.Id).ToList();
            //var annApps = SchoolLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp);
            //var apps = MasterLocator.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, BaseSecurity.IsAdminViewer(SchoolLocator.Context)));
            //Json(list.Transform(x => AnnouncementViewData.Create(x)));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult Admin(IntList gradeLevelIds)
        {
            return FakeJson("~/fakeData/adminFeed.json");
        }


        public static IList<AnnouncementViewData> GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, bool ownerOnly =false, bool? graded = null)
        {
            start = start ?? 0;
            count = count ?? 10;
            var list = schoolL.AnnouncementService.GetAnnouncements(complete, start.Value, count.Value, classId, null, ownerOnly, graded);
            var annsIdsWithApp = list.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return AnnouncementViewData.Create(list, annApps, apps);
        }
    }
}