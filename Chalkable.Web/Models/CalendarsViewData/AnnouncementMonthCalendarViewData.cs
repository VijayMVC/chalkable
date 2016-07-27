using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class AnnouncementMonthCalendarViewData : MonthCalendarViewData
    {
        public IList<ClassAnnouncementViewData> Announcements { get; set; }
        public IList<LessonPlanViewData> LessonPlans { get; set; }
        public IList<AdminAnnouncementViewData> AdminAnnouncements { get; set; } 
        public IList<SupplementalAnnouncementViewData> SupplementalAnnouncements { get; set; } 

        protected AnnouncementMonthCalendarViewData(DateTime date, bool isCurrentMonth
            , IList<ClassAnnouncement> classAnnouncements, IList<LessonPlan> lessonPlans
            , IList<AdminAnnouncement> adminAnnouncements, IList<SupplementalAnnouncement> supplementalAnnouncements, IList<ClaimInfo> claims)
            : base(date, isCurrentMonth)
        {
            Announcements = ClassAnnouncementViewData.Create(classAnnouncements, claims);
            LessonPlans = LessonPlanViewData.Create(lessonPlans);
            AdminAnnouncements = AdminAnnouncementViewData.Create(adminAnnouncements);
            SupplementalAnnouncements = SupplementalAnnouncementViewData.Create(supplementalAnnouncements);
        }

        public static AnnouncementMonthCalendarViewData Create(DateTime dateTime, bool isCurrentMonth, AnnouncementComplexList announcementList
            , IList<Date> dates, IList<ClaimInfo> claims)
        {
            var classAnnouncements = announcementList.ClassAnnouncements.Where(x => x.Expires.Date == dateTime).ToList();
            var adminAnnouncements = announcementList.AdminAnnouncements.Where(x =>x.Expires.Date == dateTime).ToList();
            var lessonPlans = announcementList.LessonPlans.Where(x => x.StartDate <= dateTime && x.EndDate >= dateTime).ToList();
            var supplementaAnns = announcementList.SupplementalAnnouncements.Where(x => x.Expires.HasValue && x.Expires.Value.Date == dateTime).ToList();
            return new AnnouncementMonthCalendarViewData(dateTime, isCurrentMonth, classAnnouncements, lessonPlans, adminAnnouncements, supplementaAnns, claims);
        }
    }
}