using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class LessonPlan : Announcement
    {
        private const string LESSON_PLAN = "Lesson Plan";

        public const string VW_LESSON_PLAN_NAME = "vwLessonPlan";
        public const string VW_LESSON_PLAN_COMPLEX = "vwLessonPlanComplex";

        public const string CLASS_REF_FIELD = "ClassRef";
        public const string SCHOOL_SCHOOLYEAR_REF_FIELD = "SchoolYearRef";
        public const string GALERRY_CATEGORY_REF_FIELD = "GalleryCategoryRef";
        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        public const string START_DATE_FIELD = "StartDate";
        public const string END_DATE_FIELD = "EndDate";
        public const string VISIBLE_FOR_STUDENT_FIELD = "VisibleForStudent";
        public const string FULL_CLASS_NAME = "FullClassName";

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? GalleryCategoryRef { get; set; }
        public int SchoolYearRef { get; set; }
        public int ClassRef { get; set; }
        public bool VisibleForStudent { get; set; }

        [NotDbFieldAttr]
        public int PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherGender { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherName { get; set; }
        [NotDbFieldAttr]
        public string ClassName { get; set; }
        [NotDbFieldAttr]
        public string FullClassName { get; set; }
        [NotDbFieldAttr]
        public override string AnnouncementTypeName
        {
            get { return LESSON_PLAN; }
        }
        [NotDbFieldAttr]
        public override AnnouncementType Type
        {
            get { return AnnouncementType.LessonPlan; }
        }
        [NotDbFieldAttr]
        public override int OwnereId
        {
            get { return PrimaryTeacherRef; }
        }
    }
}
