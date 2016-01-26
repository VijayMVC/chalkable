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
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IAnnouncementFetchService
    {
        IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, IList<int> gradeLevels, int? classId, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue);
        AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null);
        IList<Announcement> GetAnnouncementsByFilter(string filter);
        Announcement GetLastDraft();
        AnnouncementTypeEnum GetAnnouncementType(int announcementId);
        void SetSettingsForFeed(FeedSettingsInfo settings);
        FeedSettingsInfo GetSettingsForFeed();
        IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, AnnouncementTypeEnum? announcementType);
    }

    public class AnnouncementFetchService : SchoolServiceBase, IAnnouncementFetchService
    {
        public AnnouncementFetchService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }


        public IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, IList<int> gradeLevels, int? classId, FeedSettingsInfo settings, int start = 0, int count = int.MaxValue)
        {
            var feedStartDate = settings.FromDate ??  DateTime.MinValue;
            var feedEndDate = settings.ToDate ??  DateTime.MaxValue;
            
            var anns = new List<AnnouncementComplex>();
            if (BaseSecurity.IsDistrictAdmin(Context) && !classId.HasValue)
            {
                anns.AddRange(ServiceLocator.AdminAnnouncementService.GetAnnouncementsComplex(feedStartDate, feedEndDate, gradeLevels, complete, true, start, count));
            }
            var isStudent = CoreRoles.STUDENT_ROLE == Context.Role;
            //TODO: Refactor this later . . . 
            var onlyOwners = !isStudent && !(Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN) && classId.HasValue);

            if (BaseSecurity.IsTeacher(Context) || Context.Role == CoreRoles.STUDENT_ROLE || classId.HasValue)
            {
                if (!settings.AnnouncementType.HasValue)
                {
                    var ct = count != int.MaxValue ? count + 1 : count;
                    var classAnns = ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate,
                        feedEndDate, classId, complete, true, null, start, ct, settings.SortTypeEnum);

                    if (start > 0 && classAnns.Count == 0)
                        return anns;

                    //Change data range for other feed items
                    if (start > 0 && classAnns.Count > 0)
                        feedStartDate = classAnns.Min(x => x.ClassAnnouncementData.Expires);

                    if (classAnns.Count > count)
                        feedEndDate = classAnns.Max(x => x.ClassAnnouncementData.Expires);

                    //remove (count + 1) - item 
                    if (classAnns.Count > count)
                        classAnns.RemoveAt(classAnns.Count - 1);

                    anns.AddRange(classAnns);

                    if (Context.Role == CoreRoles.STUDENT_ROLE)
                    {
                        var aanns = ServiceLocator.AdminAnnouncementService.GetAnnouncementsComplex(feedStartDate, feedEndDate, gradeLevels, complete, false);
                        aanns = aanns.Where(x => x.AdminAnnouncementData.Expires < feedEndDate).ToList();
                        anns.AddRange(aanns);
                    }

                    var lps = ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate, feedEndDate, null, classId, complete, onlyOwners);
                    lps = lps.Where(x => x.LessonPlanData.EndDate < feedEndDate).ToList();
                    anns.AddRange(lps);
                }
                else switch ((AnnouncementTypeEnum) settings.AnnouncementType.Value)
                {
                    case AnnouncementTypeEnum.LessonPlan:
                        anns.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate, feedEndDate, null, classId, complete, onlyOwners, start, count));
                        break;
                    case AnnouncementTypeEnum.Class:
                        anns.AddRange(ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate, feedEndDate, classId, complete, onlyOwners, null, start, count, settings.SortTypeEnum));
                        break;
                }
            }
            anns = SortAnnouncements(anns, settings.SortTypeEnum).ToList();
            return anns;
        }

        private IList<AnnouncementComplex> SortAnnouncements(IList<AnnouncementComplex> anns, AnnouncementSortOption? sortType)
        {
            //sort all items by expires date or start date
            if (!sortType.HasValue || sortType.Value == AnnouncementSortOption.DueDateAscending)
                anns = anns.OrderBy(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
            else
                anns = anns.OrderByDescending(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();

            return anns;
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
            }
            return res;
        }
        
        public AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null)
        {
            var res = new AnnouncementComplexList
                {
                    AdminAnnouncements = new List<AdminAnnouncement>(),
                    ClassAnnouncements = new List<ClassAnnouncement>(),
                    LessonPlans = new List<LessonPlan>()
                };
            if (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AdminAnnouncements = ServiceLocator.AdminAnnouncementService.GetAdminAnnouncements(null, fromDate, toDate, studentId);

            if (classId.HasValue || !BaseSecurity.IsDistrictAdmin(Context))
            {
                res.LessonPlans = ServiceLocator.LessonPlanService.GetLessonPlans(fromDate, toDate, classId, null);
                res.ClassAnnouncements = ServiceLocator.ClassAnnouncementService.GetClassAnnouncements(fromDate, toDate, classId, onlyOwners);
            }
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
            return ServiceLocator.ClassAnnouncementService.GetLastDraft() ?? (Announcement) ServiceLocator.LessonPlanService.GetLastDraft();
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
    }
    
}
