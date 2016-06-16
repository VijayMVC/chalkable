using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class LessonPlanViewData : ShortAnnouncementViewData
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public bool HideFromStudents { get; set; }
        public int? GalleryCategoryId { get; set; }

        protected LessonPlanViewData(LessonPlan announcement)
            : base(announcement)
        {
            StartDate = announcement.StartDate;
            EndDate = announcement.EndDate;
            ClassId = announcement.ClassRef;
            ClassName = announcement.ClassName;
            FullClassName = announcement.FullClassName;
            HideFromStudents = !announcement.VisibleForStudent;
            GalleryCategoryId = announcement.GalleryCategoryRef;
            PersonId = announcement.PrimaryTeacherRef;
            PersonName = announcement.PrimaryTeacherName;
            PersonGender = announcement.PrimaryTeacherGender;
        }

        public static LessonPlanViewData Create(LessonPlan lessonPlan)
        {
            return new LessonPlanViewData(lessonPlan);
        }
        public static IList<LessonPlanViewData> Create(IList<LessonPlan> lessonPlans)
        {
            return lessonPlans.Select(Create).ToList();
        }
    }
}