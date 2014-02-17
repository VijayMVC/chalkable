using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{

    public enum StudentAnnouncementStateEnum
    {
        Auto = 0,
        None = 1,
        Manual = 2
    }
    public class StudentAnnouncement
    {
        public const string ANNOUNCEMENT_REF_FIELD_NAME = "AnnouncementRef";
        public const string GRADE_VALUE_FIELD = "GradeValue";
        public const string DROPPED_FIELD = "dropped";
        public const string STATE_FIELD = "state";

        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int PersonRef { get; set; }
        public int? GradeValue { get; set; }
        public string Comment { get; set; }
        public string ExtraCredit { get; set; }
        public bool Dropped { get; set; }
        public StudentAnnouncementStateEnum State { get; set; }
        public Guid? ApplicationRef { get; set; }

        [NotDbFieldAttr]
        public string StiScoreValue { get; set; }
        [NotDbFieldAttr]
        public int? AlternateScoreId { get; set; }
        [NotDbFieldAttr]
        public int? AlphaGradeId { get; set; }
        [NotDbFieldAttr]
        public bool Exempt { get; set; }
        [NotDbFieldAttr]
        public bool Incomplete { get; set; }
        [NotDbFieldAttr]
        public bool Late { get; set; }
        [NotDbFieldAttr]
        public bool Absent { get; set; }
    }

    public class StudentAnnouncementGrade : StudentAnnouncement
    {
        public AnnouncementComplex Announcement { get; set; }
    }

    public class StudentAnnouncementDetails : StudentAnnouncement
    {
        public int ClassId { get; set; }
        public Person Person { get; set; }
    }
}
