using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FeedController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult List(int? start, int? count, bool? complete, int? classId)
        {
            var settings = SchoolLocator.AnnouncementFetchService.GetSettingsForFeed();
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, settings));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictAdminFeed(IntList gradeLevelIds, bool? complete, int? start, int? count)
        {
            var settings = SchoolLocator.AnnouncementFetchService.GetSettingsForFeed();
            var announcements = SchoolLocator.AnnouncementFetchService.GetAnnouncementsForFeed(complete, start ?? 0, count ?? 10, gradeLevelIds, null, settings);
            return Json(PrepareFeedComplexViewData(SchoolLocator, announcements));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SetSettings(FeedSettingsInfo settings)
        {
            SchoolLocator.AnnouncementFetchService.SetSettingsForFeed(settings);
            return Json(true);
        }
        public static FeedComplexViewData GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, FeedSettingsInfo settings)
        {
            start = start ?? 0;
            count = count ?? (DemoUserService.IsDemoUser(schoolL.Context) ? int.MaxValue : 10);
            
            var list = schoolL.AnnouncementFetchService.GetAnnouncementsForFeed(complete, start.Value, count.Value, null, classId, settings);
            return PrepareFeedComplexViewData(schoolL, list);
        }

        public static FeedComplexViewData PrepareFeedComplexViewData(IServiceLocatorSchool schoolL, FeedComplex announcements)
        {
            if (DemoUserService.IsDemoUser(schoolL.Context))
                announcements.Announcements = announcements.Announcements.Where(x => x.State == AnnouncementState.Created).Take(10).ToList();
            var annsIdsWithApp = announcements.Announcements.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return new FeedComplexViewData
            {
                AnnoucementViewDatas = AnnouncementViewData.Create(announcements.Announcements, annApps, apps),
                SettingsForFeed = FeedSettingsViewData.Create(announcements.SettingsForFeed)
            };
        }

    }
}