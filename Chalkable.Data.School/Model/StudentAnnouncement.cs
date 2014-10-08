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
        public string AnnouncementTitle { get; set; }
        public int ActivityId { get; set; }
        public int StudentId { get; set; }
        public string Comment { get; set; }
        public string ExtraCredit { get; set; }
        public bool Dropped { get; set; }
        
        public decimal? NumericScore { get; set; }
        public string ScoreValue { get; set; }
        public int? AlternateScoreId { get; set; }
        public int? AlphaGradeId { get; set; }
        public bool Exempt { get; set; }
        public bool Incomplete { get; set; }
        public bool Late { get; set; }
        public string AbsenceCategory { get; set; }
        public bool Absent { get; set; }
        public bool Withdrawn { get; set; }
        public bool OverMaxScore { get; set; }


        public AlternateScore AlternateScore { get; set; }
        public bool IncludeInAverage
        {
            get { return AlternateScore == null || AlternateScore.IncludeInAverage; }
        }

        public bool IsUnexcusedAbsent
        {
            get { return Absent && !string.IsNullOrEmpty(AbsenceCategory) && AbsenceCategory.ToLower() == "u"; }
        }
        
        //TODO : remove this later
        public StudentAnnouncementStateEnum State { get { return StudentAnnouncementStateEnum.Manual; } }

        public bool IsGraded
        {
            get { return !string.IsNullOrEmpty(ScoreValue) || Late || Incomplete || Exempt || Dropped; }
        }

        public bool IncludeInTotalPoint
        {
            get { return (NumericScore.HasValue || Late) && !Incomplete && !Dropped; }
        }

        public bool? IsWithdrawn { get; set; }
    }

    public class StudentAnnouncementDetails : StudentAnnouncement
    {
        public int ClassId { get; set; }
        public Person Student { get; set; }
    }

    public class StudentAnnouncementGrade : StudentAnnouncement
    {
        private AnnouncementComplex announcement;

        public AnnouncementComplex Announcement
        {
            get { return announcement; }
            set
            {
                announcement = value;
                if (announcement != null)
                    AnnouncementId = announcement.Id;
            }
        }
    }
}
