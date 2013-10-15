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
        public Guid Id { get; set; }
        public const string ANNOUNCEMENT_REF_FIELD_NAME = "AnnouncementRef";
        public Guid AnnouncementRef { get; set; }
        public Guid ClassPersonRef { get; set; }
        public const string GRADE_VALUE_FIELD = "GradeValue";
        public int? GradeValue { get; set; }
        public string Comment { get; set; }
        public string ExtraCredit { get; set; }
        public const string DROPPED_FIELD = "dropped"; 
        public bool Dropped { get; set; }
        public const string STATE_FIELD = "state";
        public StudentAnnouncementStateEnum State { get; set; }
        public Guid? ApplicationRef { get; set; }


    }

    public class StudentAnnouncementGrade : StudentAnnouncement
    {
        public AnnouncementComplex Announcement { get; set; }
    }

    public class StudentAnnouncementDetails : StudentAnnouncement
    {
        public Guid ClassId { get; set; }
        public Person Person { get; set; }
    }
}
