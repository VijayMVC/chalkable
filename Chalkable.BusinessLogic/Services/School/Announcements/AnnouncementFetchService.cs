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
        FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, DateTime?startDate, DateTime?endDate, bool?lessonPlansOnly, bool?sortType);
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


        public FeedComplex GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId, DateTime? startDate, DateTime? endDate, bool? lessonPlansOnly, bool? sortType)
        {
            //get or set settings
            DateTime fromDate, toDate;
            bool onlyLessonPlans, sort;
            if (startDate == null)
                GetSettingsForFeed(out fromDate, out toDate, out onlyLessonPlans, out sort);
            else
            {
                fromDate = startDate.Value;
                toDate = endDate.Value;
                onlyLessonPlans = lessonPlansOnly.Value;
                sort = sortType.Value;
                ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value,
                    new Dictionary<string, object>()
                    {
                        {PersonSetting.FEED_START_DATE, fromDate},
                        {PersonSetting.FEED_END_DATE, toDate},
                        {PersonSetting.FEED_LESSON_PLANS_ONLY, onlyLessonPlans},
                        {PersonSetting.FEED_SORTING, sort }
                    });
            }

            if (BaseSecurity.IsDistrictAdmin(Context))
                return ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null, fromDate, toDate, start, count, sort);

            var res = new List<AnnouncementComplex>();
            if (BaseSecurity.IsTeacher(Context) || Context.Role == CoreRoles.STUDENT_ROLE)
            {
                if (!onlyLessonPlans)
                {
                    var classAnns = ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(fromDate, toDate,
                        classId, complete, true, null, start, count + 1);

                    if (start > 0 && classAnns.Count == 0)
                        return new FeedComplex();

                    //remove (count + 1) - item 
                    if (classAnns.Count > count)
                        classAnns.RemoveAt(classAnns.Count - 1);

                    res.AddRange(classAnns);

                    if (Context.Role == CoreRoles.STUDENT_ROLE)
                        res.AddRange(ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null,
                            fromDate, toDate, ownedOnly: false).Announcements);
                }
                //get addtional feed items 
                res.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(fromDate, toDate, null, classId, complete, true));
                
            }
            //sort all items by expires date or start date
            var fc = new FeedComplex
            {
                ToDate = toDate,
                FromDate = fromDate,
                LessonPlansOnly = onlyLessonPlans,
                SortType = sort,
            };
            if (!sort)
                fc.Announcements = res.OrderBy(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
            else
                fc.Announcements = res.OrderByDescending(x =>
                {
                    if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
            return fc;
        }

        private void GetSettingsForFeed(out DateTime fromDate, out DateTime toDate, out bool lessonPlansOnly, out bool sortType)
        {
            var query = new List<string>
            {
                PersonSetting.FEED_START_DATE,
                PersonSetting.FEED_END_DATE,
                PersonSetting.FEED_LESSON_PLANS_ONLY,
                PersonSetting.FEED_SORTING
            };
            var settings = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value, query);
            var startDate = settings.FirstOrDefault(x => x.Key == PersonSetting.FEED_START_DATE);
            var endDate = settings.FirstOrDefault(x => x.Key == PersonSetting.FEED_END_DATE);
            var lpOnly = settings.FirstOrDefault(x => x.Key == PersonSetting.FEED_LESSON_PLANS_ONLY);
            var sort = settings.FirstOrDefault(x => x.Key == PersonSetting.FEED_SORTING);
            if (startDate == null)
            {
                fromDate = Context.SchoolYearStartDate ?? DateTime.MinValue;
                ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value,
                    new Dictionary<string, object>()
                    {
                        {PersonSetting.FEED_START_DATE, Context.SchoolYearStartDate ?? DateTime.MinValue}
                    });
            }
            else fromDate = DateTime.ParseExact(startDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (endDate == null)
            {
                toDate = Context.SchoolYearEndDate ?? DateTime.MaxValue;
                ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value,
                    new Dictionary<string, object>()
                    {
                        {PersonSetting.FEED_END_DATE, Context.SchoolYearEndDate ?? toDate}
                    });
            }
            else toDate = DateTime.ParseExact(endDate.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (lpOnly == null)
            {
                lessonPlansOnly = false;
                ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value,
                    new Dictionary<string, object>()
                    {
                        {PersonSetting.FEED_LESSON_PLANS_ONLY, lessonPlansOnly}
                    }); 
            }
            else lessonPlansOnly = bool.Parse(lpOnly.Value);
            if (sort == null)
            {
                sortType = false;
                ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value,
                    new Dictionary<string, object>()
                    {
                        {PersonSetting.FEED_SORTING, sortType}
                    });
            }
            else sortType = bool.Parse(sort.Value);
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
        public bool? LessonPlansOnly { get; set; }
        public bool? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
