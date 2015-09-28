using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IAnnouncementFetchService
    {
        FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, FeedSettings settings);
        AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null);
        IList<Announcement> GetAnnouncementsByFilter(string filter);
        Announcement GetLastDraft();
        AnnouncementType GetAnnouncementType(int announcementId);
    }

    public class AnnouncementFetchService : SchoolServiceBase, IAnnouncementFetchService
    {
        public AnnouncementFetchService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }


        public FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, FeedSettings settings)
        {
            DateTime feedStartDate;
            DateTime feedEndDate;

            //get or set settings
            if (!settings.AnnouncementType.HasValue && !settings.FromDate.HasValue && !settings.ToDate.HasValue && !settings.GradingPeriodId.HasValue)
                GetSettingsForFeed(out settings);
            else
                SetSettingsForFeed(settings);

            //if items should be from certain grading period
            if (settings.GradingPeriodId.HasValue)
            {
                var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(settings.GradingPeriodId.Value);
                feedStartDate = gp.StartDate;
                feedEndDate = gp.EndDate;
            }
            else
            {
                feedStartDate = settings.FromDate.Value;
                feedEndDate = settings.ToDate.Value;
            }

            if (BaseSecurity.IsDistrictAdmin(Context))
                return ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null,
                    settings, start, count);
            
            var res = new FeedComplex
            {
                SettingsForFeed = settings,
                Announcements = new List<AnnouncementComplex>()
            };
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
                        anns.AddRange(ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null,
                            settings, ownedOnly: false).Announcements);
                    anns.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate,
                        feedEndDate, null, classId, complete, true));
                }
                else switch ((AnnouncementType) settings.AnnouncementType.Value)
                {
                    case AnnouncementType.LessonPlan:
                        anns.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(feedStartDate, feedEndDate, null, classId,
                            complete, true, start, count));
                        break;
                    case AnnouncementType.Class:
                        anns.AddRange(ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(feedStartDate, feedEndDate,
                            classId, complete, true, null, start, count));
                        break;
                }
                res.Announcements = anns;
            }

            //sort all items by expires date or start date
            if (!settings.SortType.Value)
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

        private void GetSettingsForFeed(out FeedSettings settings)
        {
            settings = new FeedSettings();
            var query = new List<string>
            {
                PersonSetting.FEED_START_DATE,
                PersonSetting.FEED_END_DATE,
                PersonSetting.FEED_ANNOUNCEMENT_TYPE,
                PersonSetting.FEED_SORTING,
                PersonSetting.FEED_GRADING_PERIOD_ID
            };

            var sett = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value, query);
            var startDate = sett.FirstOrDefault(x => x.Key == PersonSetting.FEED_START_DATE);
            var endDate = sett.FirstOrDefault(x => x.Key == PersonSetting.FEED_END_DATE);
            var annoncementType = sett.FirstOrDefault(x => x.Key == PersonSetting.FEED_ANNOUNCEMENT_TYPE);
            var sort = sett.FirstOrDefault(x => x.Key == PersonSetting.FEED_SORTING);
            var grPeriodId = sett.FirstOrDefault(x => x.Key == PersonSetting.FEED_GRADING_PERIOD_ID);

            if (grPeriodId?.Value != null)
            {
                settings.GradingPeriodId = int.Parse(grPeriodId.Value);
                settings.FromDate = null;
                settings.ToDate = null;
            }
            else if (startDate?.Value != null && endDate?.Value != null)
            {
                settings.FromDate = DateTime.ParseExact(startDate.Value, Constants.DATE_FORMAT,
                    CultureInfo.InvariantCulture);
                settings.ToDate = DateTime.ParseExact(endDate.Value, Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
                settings.GradingPeriodId = null;
            }
            else
            {
                settings.FromDate = Context.SchoolYearStartDate ?? DateTime.MinValue;
                settings.ToDate = Context.SchoolYearEndDate ?? DateTime.MaxValue;
                settings.GradingPeriodId = null;
            }

            if (annoncementType == null)
                settings.AnnouncementType = null;
            else settings.AnnouncementType = annoncementType.Value != null ? int.Parse(annoncementType.Value): (int?) null;

            settings.SortType = sort != null && bool.Parse(sort.Value);
        }

        private void SetSettingsForFeed(FeedSettings settings)
        {
            ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value, new Dictionary<string, object>()
            {
                {PersonSetting.FEED_ANNOUNCEMENT_TYPE, settings.AnnouncementType },
                {PersonSetting.FEED_START_DATE, settings.FromDate },
                {PersonSetting.FEED_END_DATE, settings.ToDate },
                {PersonSetting.FEED_GRADING_PERIOD_ID, settings.GradingPeriodId },
                {PersonSetting.FEED_SORTING, settings.SortType }
            });
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

        public AnnouncementType GetAnnouncementType(int announcementId)
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
        public FeedSettings SettingsForFeed { get; set; }
    }

    public class FeedSettings
    {
        public int? AnnouncementType { get; set; }
        public bool? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? GradingPeriodId { get; set; }
    }
}
