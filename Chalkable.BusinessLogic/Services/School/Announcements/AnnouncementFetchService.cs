using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IAnnouncementFetchService
    {
        IList<AnnouncementComplex> GetAnnouncementsForAdminFeed(bool? complete, IList<int> gradeLevels, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue);
        IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int? classId, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue);
        AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null, int? teacherId = null, bool filterByStartDate = true);
        IList<Announcement> GetAnnouncementsByFilter(string filter);
        Announcement GetLastDraft();
        AnnouncementTypeEnum GetAnnouncementType(int announcementId);
        void SetSettingsForFeed(FeedSettingsInfo settings);
        FeedSettingsInfo GetSettingsForFeed();
        FeedSettingsInfo GetSettingsForClassFeed(int classId);
        IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, AnnouncementTypeEnum? announcementType);
    }
    
    public class AnnouncementFetchService : SchoolServiceBase, IAnnouncementFetchService
    {
        public AnnouncementFetchService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }
        
        private static IDictionary<AnnouncementSortOption, IFeedItemHandler> _handlers
            = new Dictionary<AnnouncementSortOption, IFeedItemHandler>
            {
                [AnnouncementSortOption.DueDateAscending] = new FeedItemsSortedByDueDateHandler(false),
                [AnnouncementSortOption.DueDateDescending] = new FeedItemsSortedByDueDateHandler(true),
                [AnnouncementSortOption.NameAscending] = new FeedItemsSortByTitleHandler(false),
                [AnnouncementSortOption.NameDescending] = new FeedItemsSortByTitleHandler(true),
                [AnnouncementSortOption.SectionNameAscending] = new FeedItemsSortedByClassNameHandler(false),
                [AnnouncementSortOption.SectionNameDescending] = new FeedItemsSortedByClassNameHandler(true)
            };


        public IList<AnnouncementComplex> GetAnnouncementsForAdminFeed(bool? complete, IList<int> gradeLevels, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue)
        {
            var feedStartDate = settings.FromDate ?? DateTime.MinValue;
            var feedEndDate = settings.ToDate ?? DateTime.MaxValue;
            var sortOption = settings.SortTypeEnum ?? AnnouncementSortOption.DueDateAscending;

            return _handlers[sortOption].GetAdminAnnouncementsOnly(ServiceLocator, feedStartDate, feedEndDate, gradeLevels, complete, start, count);
        }

        public IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int? classId, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue)
        {
            var feedStartDate = settings.FromDate ??  DateTime.MinValue;
            var feedEndDate = settings.ToDate ??  DateTime.MaxValue;
            var sortOption = settings.SortTypeEnum ?? AnnouncementSortOption.DueDateAscending;
            var ownedOnly = !classId.HasValue ? (bool?)true : null;

            if (!settings.AnnouncementTypeEnum.HasValue)
                return _handlers[sortOption].GetAllItems(ServiceLocator, feedStartDate, feedEndDate, classId, complete, start, count, ownedOnly);

            switch (settings.AnnouncementTypeEnum.Value)
            {
                case AnnouncementTypeEnum.Class:
                    return ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate, feedEndDate, classId, complete, null, start, count, sortOption);
                case AnnouncementTypeEnum.LessonPlan:
                    return _handlers[sortOption].GetLessonPlansOnly(ServiceLocator, feedStartDate, feedEndDate, classId, complete, start, count, ownedOnly);
                case AnnouncementTypeEnum.Admin:
                    return _handlers[sortOption].GetAdminAnnouncementsOnly(ServiceLocator, feedStartDate, feedEndDate, null, complete, start, count);
                case AnnouncementTypeEnum.Supplemental:
                    return _handlers[sortOption].GetSupplementalAnnouncementsOnly(ServiceLocator, feedStartDate, feedEndDate, classId, complete, start, count, true);
            }
            return new List<AnnouncementComplex>();
        }

      
        public FeedSettingsInfo GetSettingsForFeed()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var query = new List<string>
            {
                PersonSetting.FEED_START_DATE,
                PersonSetting.FEED_END_DATE,
                PersonSetting.FEED_ANNOUNCEMENT_TYPE,
                PersonSetting.FEED_SORTING,
                PersonSetting.FEED_GRADING_PERIOD_ID
            };
            var settings = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value, Context.SchoolYearId.Value, query);
            var res = new FeedSettingsInfo(settings);
            if (res.GradingPeriodId.HasValue)
            {
                var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(res.GradingPeriodId.Value);
                res.FromDate = gp.StartDate;
                res.ToDate = gp.EndDate;
            }
            else if(res.AnyDate)
            {
                res.FromDate = Context.SchoolYearStartDate;
                res.ToDate = Context.SchoolYearEndDate;
            }
            return res;
        }

        public FeedSettingsInfo GetSettingsForClassFeed(int classId)
        {
            var c = ServiceLocator.ClassService.GetClassDetailsById(classId);
            return new FeedSettingsInfo
            {
                AnyDate = true,
                FromDate = c.SchoolYear.StartDate,
                ToDate = c.SchoolYear.EndDate
            };
        }

        public void SetSettingsForFeed(FeedSettingsInfo settings)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value, Context.SchoolYearId.Value, settings.ToDictionary());
        }


        public IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, AnnouncementTypeEnum? announcementType)
        {
            var res = new List<AnnouncementDetails>();
            if (!BaseSecurity.IsTeacher(Context) && !classId.HasValue )
            {
                if(!announcementType.HasValue || announcementType == AnnouncementTypeEnum.Admin)
                    res.AddRange(ServiceLocator.AdminAnnouncementService.GetAnnouncementDetailses(fromDate, toDate, null, complete, true));
            }
            if (!BaseSecurity.IsDistrictAdmin(Context) || classId.HasValue)
            {
                var onlyOwners = !Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);

                if (!announcementType.HasValue || announcementType == AnnouncementTypeEnum.Class)
                    res.AddRange(ServiceLocator.ClassAnnouncementService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));

                if (!announcementType.HasValue || announcementType == AnnouncementTypeEnum.LessonPlan)
                    res.AddRange(ServiceLocator.LessonPlanService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));

                if(!announcementType.HasValue || announcementType == AnnouncementTypeEnum.Supplemental)
                    res.AddRange(ServiceLocator.SupplementalAnnouncementService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));
            }
            return res;
        }
        
        public AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, 
            int? studentId = null, int? teacherId = null, bool filterByStartDate = true)
        {
            var res = new AnnouncementComplexList
                {
                    AdminAnnouncements = new List<AdminAnnouncement>(),
                    ClassAnnouncements = new List<ClassAnnouncement>(),
                    LessonPlans = new List<LessonPlan>(),
                    SupplementalAnnouncements = new List<SupplementalAnnouncement>()
                };

            if (classId.HasValue || studentId.HasValue || teacherId.HasValue || !BaseSecurity.IsDistrictAdmin(Context))
            {
                res.LessonPlans = ServiceLocator.LessonPlanService.GetLessonPlans(fromDate, toDate, classId, studentId, teacherId, filterByStartDate);
                res.ClassAnnouncements = ServiceLocator.ClassAnnouncementService.GetClassAnnouncements(fromDate, toDate, classId, studentId, teacherId);
                res.SupplementalAnnouncements = ServiceLocator.SupplementalAnnouncementService.GetSupplementalAnnouncements(fromDate, toDate, classId, studentId, teacherId);
            }
            if(!classId.HasValue && !teacherId.HasValue && (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role))
                res.AdminAnnouncements = ServiceLocator.AdminAnnouncementService.GetAdminAnnouncements(null, fromDate, toDate, studentId);
            return res;
        }

        public IList<Announcement> GetAnnouncementsByFilter(string filter)
        {
            var res = new List<Announcement>();
            if (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AddRange(ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsByFilter(filter));
            if (BaseSecurity.IsTeacher(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
            {
                res.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansbyFilter(filter));
                res.AddRange(ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsByFilter(filter));
            }
            return res.OrderBy(x => x.Created).ToList();
        }

        public Announcement GetLastDraft()
        {
            if (BaseSecurity.IsDistrictAdmin(Context))
                return ServiceLocator.AdminAnnouncementService.GetLastDraft();
            return ServiceLocator.ClassAnnouncementService.GetLastDraft() 
                ?? (Announcement) ServiceLocator.LessonPlanService.GetLastDraft() 
                ?? ServiceLocator.SupplementalAnnouncementService.GetLastDraft();
        }

        public AnnouncementTypeEnum GetAnnouncementType(int announcementId)
        {
            return DoRead(u => new AnnouncementDataAccess(u).GetAnnouncementType(announcementId));
        }
    }

    public class AnnouncementComplexList
    {
        public IList<LessonPlan> LessonPlans { get; set; }
        public IList<ClassAnnouncement> ClassAnnouncements { get; set; }
        public IList<AdminAnnouncement> AdminAnnouncements { get; set; } 
        public IList<SupplementalAnnouncement> SupplementalAnnouncements { get; set; } 
    }
    
}
