using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IAnnouncementFetchService
    {
        FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, IList<int> gradeLevels, int? classId, FeedSettingsInfo settings);
        AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null);
        IList<Announcement> GetAnnouncementsByFilter(string filter);
        Announcement GetLastDraft();
        AnnouncementTypeEnum GetAnnouncementType(int announcementId);
        void SetSettingsForFeed(FeedSettingsInfo settings);
        FeedSettingsInfo GetSettingsForFeed();
        IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, bool onlyOwners = true);
    }

    public class AnnouncementFetchService : SchoolServiceBase, IAnnouncementFetchService
    {
        public AnnouncementFetchService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }


        public FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, IList<int> gradeLevels, int? classId, FeedSettingsInfo settings)
        {
            var feedStartDate = settings.FromDate ??  DateTime.MinValue;
            var feedEndDate = settings.ToDate ??  DateTime.MaxValue;
            
            var res = new FeedComplex
            {
                SettingsForFeed = settings,
                Announcements = new List<AnnouncementComplex>()
            };
            if (BaseSecurity.IsDistrictAdmin(Context))
                res.Announcements = ServiceLocator.AdminAnnouncementService.GetAnnouncementsComplex(feedStartDate, feedEndDate, gradeLevels, complete, true, start, count);
            
            if (BaseSecurity.IsTeacher(Context) || Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var anns = new List<AnnouncementComplex>();
                if (!settings.AnnouncementType.HasValue)
                {
                    var classAnns =
                        ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate,
                            feedEndDate, classId, complete, true, null, start, count + 1);

                    if (start > 0 && classAnns.Count == 0)
                        return res;

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
                        anns.AddRange(ServiceLocator.AdminAnnouncementService.GetAnnouncementsComplex(feedStartDate, feedEndDate, gradeLevels, complete, false));
                    anns.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate,
                        feedEndDate, null, classId, complete, true));
                }
                else switch ((AnnouncementTypeEnum) settings.AnnouncementType.Value)
                {
                    case AnnouncementTypeEnum.LessonPlan:
                        anns.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate, feedEndDate, null, classId,
                            complete, true, start, count));
                        break;
                    case AnnouncementTypeEnum.Class:
                        anns.AddRange(ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate, feedEndDate,
                            classId, complete, true, null, start, count));
                        break;
                }
                res.Announcements = anns;
            }

            //sort all items by expires date or start date
            if (!settings.SortType.HasValue || !settings.SortType.Value)
                res.Announcements = res.Announcements.OrderBy(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
            else
                res.Announcements = res.Announcements.OrderByDescending(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
            return res;
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

        public IList<AnnouncementDetails> GetAnnouncementDetailses(DateTime? fromDate, DateTime? toDate, int? classId, bool? complete, bool onlyOwners = true)
        {
            var res = new List<AnnouncementDetails>();
            if(CoreRoles.DISTRICT_ADMIN_ROLE == Context.Role || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AddRange(ServiceLocator.AdminAnnouncementService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));
            if (CoreRoles.DISTRICT_ADMIN_ROLE != Context.Role)
            {
                res.AddRange(ServiceLocator.ClassAnnouncementService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));
                res.AddRange(ServiceLocator.LessonPlanService.GetAnnouncementDetailses(fromDate, toDate, classId, complete, onlyOwners));
            }
            return res;
        }

        

        public AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null)
        {
            var res = new AnnouncementComplexList()
                {
                    AdminAnnouncements = new List<AdminAnnouncement>(),
                    ClassAnnouncements = new List<ClassAnnouncement>(),
                    LessonPlans = new List<LessonPlan>()
                };
            if (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AdminAnnouncements = ServiceLocator.AdminAnnouncementService.GetAdminAnnouncements(null, fromDate, toDate, studentId);
            if (BaseSecurity.IsTeacher(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
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

    public class FeedComplex
    {
        public IList<AnnouncementComplex> Announcements { get; set; }
        public FeedSettingsInfo SettingsForFeed { get; set; }
    }
}
