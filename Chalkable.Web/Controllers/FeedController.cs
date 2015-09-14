using System;
using System.Linq;
using System.Web.Mvc;
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
        public ActionResult List(int? start, int? count, bool? complete, int? classId, DateTime? startDate, 
            DateTime? endDate, bool? lessonPlansOnly, bool? sortType)
        {
            return
                Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, startDate, endDate,
                    lessonPlansOnly, sortType));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictAdminFeed(IntList gradeLevelIds, bool? complete, int? start, int? count, DateTime? startDate,
            DateTime? endDate, bool? sortType)
        {
            var announcements = SchoolLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete,
                gradeLevelIds, startDate, endDate, start ?? 0, count ?? 10, sortType: sortType);
            return Json(GetAnnouncementForFeedList(SchoolLocator, announcements));
        }

        public static FeedComplexViewData GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, DateTime? startDate, DateTime? endDate, bool?lessonPlansOnly, bool? sortType)
        {
            start = start ?? 0;
            count = count ?? (DemoUserService.IsDemoUser(schoolL.Context) ? int.MaxValue : 10);
            var list = schoolL.AnnouncementFetchService.GetAnnouncementsForFeed(complete, start.Value, count.Value, classId, startDate, endDate, lessonPlansOnly, sortType);
            return GetAnnouncementForFeedList(schoolL, list);
        }

        public static FeedComplexViewData GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, FeedComplex announcements)
        {
            if (DemoUserService.IsDemoUser(schoolL.Context))
                announcements.Announcements = announcements.Announcements.Where(x => x.State == AnnouncementState.Created).Take(10).ToList();
            var annsIdsWithApp = announcements.Announcements.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return new FeedComplexViewData()
            {
                AnnoucementViewDatas = AnnouncementViewData.Create(announcements.Announcements, annApps, apps),
                LessonPlansOnly = announcements.LessonPlansOnly,
                SortType = announcements.SortType,
                FromDate = announcements.FromDate,
                ToDate = announcements.ToDate
            };
        }

    }
}