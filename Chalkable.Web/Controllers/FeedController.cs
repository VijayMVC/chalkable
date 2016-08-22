using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Master;
using Chalkable.BusinessLogic.Services.School;
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
        public ActionResult List(int? start, int? count, bool? complete, int? classId, IList<AnnouncementInputModel> createdAnnouncements)
        {
            var settings = SchoolLocator.AnnouncementFetchService.GetSettingsForFeed();
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, settings, createdAnnouncements));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Announcement })]
        public ActionResult ClassFeed(int classId, int? start, int? count, bool? complete, FeedSettingsInfo settings)
        {
            //settings = SchoolLocator.AnnouncementFetchService.GetSettingsForClassFeed(classId);
            return Json(GetAnnouncementForFeedList(SchoolLocator, start, count, complete, classId, settings, new List<AnnouncementInputModel>()));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DistrictAdminFeed(IntList gradeLevelIds, bool? complete, int? start, int? count)
        {
            var settings = SchoolLocator.AnnouncementFetchService.GetSettingsForFeed();
            var announcements = SchoolLocator.AnnouncementFetchService.GetAnnouncementsForAdminFeed(complete,  gradeLevelIds, settings, start ?? 0, count ?? 10);
            
            return Json(PrepareFeedComplexViewData(SchoolLocator, announcements, settings, null));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult SetSettings(FeedSettingsInfo settings)
        {
            SchoolLocator.AnnouncementFetchService.SetSettingsForFeed(settings);
            return Json(true);
        }
        public static FeedComplexViewData GetAnnouncementForFeedList(IServiceLocatorSchool schoolL, int? start, int? count
            , bool? complete, int? classId, FeedSettingsInfo settings, IList<AnnouncementInputModel> createdAnnouncements)
        {
            start = start ?? 0;
            count = count ?? (DemoUserService.IsDemoUser(schoolL.Context) ? int.MaxValue : 10);
            
            var list = schoolL.AnnouncementFetchService.GetAnnouncementsForFeed(complete, classId, settings, start.Value, count.Value);
            List<AnnouncementComplex> createdAnns = new List<AnnouncementComplex>();
            if (createdAnnouncements != null)
            {
                var lpIds = createdAnnouncements.Where(x => x.AnnouncementType == (int) AnnouncementTypeEnum.LessonPlan)
                    .Select(x => x.AnnouncementId).ToList();
                var clAnnIds = createdAnnouncements.Where(x => x.AnnouncementType == (int) AnnouncementTypeEnum.Class)
                    .Select(x => x.AnnouncementId).ToList();

                if(clAnnIds.Count > 0)
                    createdAnns.AddRange(schoolL.ClassAnnouncementService.GetAnnouncementsByIds(clAnnIds));
                if(lpIds.Count > 0)
                    createdAnns.AddRange(schoolL.LessonPlanService.GetAnnouncementsByIds(lpIds));
            }
            return PrepareFeedComplexViewData(schoolL, list, settings, createdAnns);
        }

        public static FeedComplexViewData PrepareFeedComplexViewData(IServiceLocatorSchool schoolL, IList<AnnouncementComplex> announcements
            , FeedSettingsInfo settings, IList<AnnouncementComplex> createdAnnouncements)
        {
            return new FeedComplexViewData
            {
                AnnoucementViewDatas = PrepareAnnouncementsComplexViewData(schoolL, announcements),
                CreatedAnnouncements = PrepareAnnouncementsComplexViewData(schoolL, createdAnnouncements ?? new List<AnnouncementComplex>()),
                SettingsForFeed = FeedSettingsViewData.Create(settings)
            };
        }

        public static IList<AnnouncementViewData> PrepareAnnouncementsComplexViewData(IServiceLocatorSchool schoolL, IList<AnnouncementComplex> announcements)
        {
            if (DemoUserService.IsDemoUser(schoolL.Context))
                announcements = announcements.Where(x => x.State == AnnouncementState.Created).Take(10).ToList();
            var annsIdsWithApp = announcements.Where(x => x.ApplicationCount == 1).Select(x => x.Id).ToList();
            var annApps = schoolL.ApplicationSchoolService.GetAnnouncementApplicationsByAnnIds(annsIdsWithApp, true);
            var apps = schoolL.ServiceLocatorMaster.ApplicationService.GetApplicationsByIds(annApps.Select(x => x.ApplicationRef).ToList());
            annApps = annApps.Where(x => apps.Any(a => a.Id == x.ApplicationRef)).ToList();
            return AnnouncementViewData.Create(announcements, annApps, apps, schoolL.Context.Claims);
        }
    }
}