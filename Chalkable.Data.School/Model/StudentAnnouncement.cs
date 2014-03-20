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
        public int AnnouncementId { get; set; }
        public int ActivityId { get; set; }
        public int StudentId { get; set; }
        public string Comment { get; set; }
        public string ExtraCredit { get; set; }
        public bool Dropped { get; set; }
        
        public int? NumericScore { get; set; }
        public string ScoreValue { get; set; }
        public int? AlternateScoreId { get; set; }
        public int? AlphaGradeId { get; set; }
        public bool Exempt { get; set; }
        public bool Incomplete { get; set; }
        public bool Late { get; set; }
        public bool Absent { get; set; }
        public bool Withdrawn { get; set; }
        public bool OverMaxScore { get; set; }

        //TODO : remove this later
        public StudentAnnouncementStateEnum State { get { return StudentAnnouncementStateEnum.Manual; } }
    }

    public class StudentAnnouncementDetails : StudentAnnouncement
    {
        public int ClassId { get; set; }
        public Person Student { get; set; }
    }

    public class StudentAnnouncementGrade : StudentAnnouncement
    {
        public AnnouncementComplex Announcement { get; set; }
    }
}
