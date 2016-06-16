using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class LessonPlan : Announcement
    {
        public const string VW_LESSON_PLAN_NAME = "vwLessonPlan";
        public const string VW_LESSON_PLAN_COMPLEX = "vwLessonPlanComplex";

        public const string CLASS_REF_FIELD = nameof(ClassRef);
        public const string SCHOOL_SCHOOLYEAR_REF_FIELD = nameof(SchoolYearRef);
        public const string GALERRY_CATEGORY_REF_FIELD = nameof(GalleryCategoryRef);
        public const string PRIMARY_TEACHER_REF_FIELD = nameof(PrimaryTeacherRef);
        public const string START_DATE_FIELD = nameof(StartDate);
        public const string END_DATE_FIELD = nameof(EndDate);
        public const string VISIBLE_FOR_STUDENT_FIELD = nameof(VisibleForStudent);
        public const string FULL_CLASS_NAME = nameof(FullClassName);
        public const string IN_GALLERY = nameof(InGallery);

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? GalleryCategoryRef { get; set; }
        public int SchoolYearRef { get; set; }
        public int? ClassRef { get; set; }
        public bool VisibleForStudent { get; set; }
        public bool InGallery { get; set; }

        [NotDbFieldAttr]
        public int? PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherGender { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherName { get; set; }
        [NotDbFieldAttr]
        public string ClassName { get; set; }
        [NotDbFieldAttr]
        public string FullClassName { get; set; }
        [NotDbFieldAttr]
        public override string AnnouncementTypeName => "Lesson Plan";
        [NotDbFieldAttr]
        public override AnnouncementTypeEnum Type => AnnouncementTypeEnum.LessonPlan;
        [NotDbFieldAttr]
        public override int? OwnereId => PrimaryTeacherRef;
    }
}
