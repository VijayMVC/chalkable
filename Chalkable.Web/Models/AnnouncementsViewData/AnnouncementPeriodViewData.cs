using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementPeriodViewData
    {
        public ScheduleItemViewData Period { get; set; }
        public IList<ClassAnnouncementViewData> Announcements { get; set; }
        public IList<LessonPlanViewData> LessonPlans { get; set; }
        public IList<SupplementalAnnouncementViewData> SupplementalAnnouncements { get; set; }
         
        public static AnnouncementPeriodViewData Create(ScheduleItem scheduleItem, IList<ClassAnnouncement> classAnnouncements,
            IList<LessonPlan> lessonPlans, IList<SupplementalAnnouncement> supplementalAnnouncements, DateTime nowSchoolTime)
        {
            return new AnnouncementPeriodViewData
            {
                Period =  ScheduleItemViewData.Create(scheduleItem, nowSchoolTime),
                Announcements = ClassAnnouncementViewData.Create(classAnnouncements),
                LessonPlans = LessonPlanViewData.Create(lessonPlans),
                SupplementalAnnouncements = SupplementalAnnouncementViewData.Create(supplementalAnnouncements)
            };
        }
        
        public static IList<AnnouncementPeriodViewData> Create(IList<ScheduleItem> schedule, IList<ClassAnnouncement> classAnnouncements, 
            IList<LessonPlan> lessonPlans, IList<SupplementalAnnouncement> supplementalAnnouncements, DateTime nowSchoolTime)
        {
            var res = new List<AnnouncementPeriodViewData>();
            foreach (var scheduleItem in schedule)
            {
                var annItems = classAnnouncements.Where(x => scheduleItem.ClassId == x.ClassRef).ToList();
                var lsPlans = lessonPlans.Where(x => scheduleItem.ClassId == x.ClassRef).ToList();
                var suppAnns = supplementalAnnouncements.Where(x => scheduleItem.ClassId == x.ClassRef).ToList();
                res.Add(Create(scheduleItem, annItems, lsPlans, suppAnns, nowSchoolTime));
            }
            return res;
        }
    }
}