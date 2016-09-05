using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public class ClassAnnouncement : Announcement
    {
        public const string VW_CLASS_ANNOUNCEMENT_NAME = "vwClassAnnouncement";
        public const string VW_CLASS_ANNOUNCEMENT_COMPLEX = "vwClassAnnouncementComplex";

        public const string EXPIRES_FIELD = "Expires";
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string CLASS_ANNOUNCEMENT_TYPE_REF_FIELD = "ClassAnnouncementTypeRef";
        public const string SIS_ACTIVITY_ID_FIELD = "SisActivityId";
        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        public const string SCHOOL_SCHOOLYEAR_REF_FIELD = "SchoolYearRef";

        public const string VISIBLE_FOR_STUDENT = "VisibleForStudent";

        public const string FULL_CLASS_NAME = "FullClassName";
        
        //todo remove this later

        public const int DEFAULT_MAX_SCORE = 100;
        public const int DEFAULT_WEIGHT_ADDITION = 0;
        public const int DEFAULT_WEGIHT_MULTIPLIER = 1;
        public const bool DEFAULT_IS_SCORED = true;

        public DateTime Expires { get; set; }
        public int? ClassAnnouncementTypeRef { get; set; }
        public int ClassRef { get; set; }
        public int SchoolYearRef { get; set; }
        public bool Dropped { get; set; }
        
        public int? SisActivityId { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool VisibleForStudent { get; set; }
        public bool MayBeDropped { get; set; }
        public bool IsScored { get; set; }
        
        [NotDbFieldAttr]
        public override bool IsSubmitted => SisActivityId.HasValue && base.IsSubmitted;
        [NotDbFieldAttr]
        public override int? OwnereId => PrimaryTeacherRef;

        [NotDbFieldAttr]
        public override string AnnouncementTypeName => ClassAnnouncementTypeName;

        [NotDbFieldAttr]
        public override AnnouncementTypeEnum Type => AnnouncementTypeEnum.Class;

        [NotDbFieldAttr]
        public bool MayBeExempt { get; set; }
        [NotDbFieldAttr]
        public int PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherGender { get; set; }
        [NotDbFieldAttr]
        public string PrimaryTeacherName { get; set; }

        [NotDbFieldAttr]
        public string ClassAnnouncementTypeName { get; set; }
        [NotDbFieldAttr]
        public int? ChalkableAnnouncementType { get; set; }
        
        [NotDbFieldAttr]
        public string ClassName { get; set; }
        [NotDbFieldAttr]
        public string FullClassName { get; set; }
        [NotDbFieldAttr]
        public Guid? DepartmentId { get; set; }
    }
}
