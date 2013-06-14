using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{

    public enum AnnouncementState
    {
        Draft = 0,
        Created = 1
    }


    public enum GradingStyleEnum
    {
        Numeric100 = 0,
        Abcf = 1,
        Complete = 2,
        Check = 3
    }

    public class Announcement
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public Guid AnnouncementTypeRef { get; set; }
        public AnnouncementState State { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
        public string Subject { get; set; }
        public Guid MarkingPeriodClassRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }
    }


    public class AnnouncementComplex : Announcement
    {
        public string AnnouncementTypeName { get; set; }
        public string PersonName { get; set; }
        public string Gender { get; set; }

        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid GradeLevelId { get; set; }
        public Guid CourseId { get; set; }

        public Guid MarkingPeriodId { get; set; }

        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWidthAttachments { get; set; }
        public int GradingsStudentsCount { get; set; }
        public int? Avg { get; set; }
        public int ApplicationCount { get; set; }

        public bool IsOwner { get; set; }
        public Guid? RecipientDataPersonId { get; set; }
        public bool Starred { get; set; }
        
        public bool IsDraft
        {
            get { return State == AnnouncementState.Draft; }
        }
    }

    public class AnnouncementDetails : AnnouncementComplex
    {
        public bool IsGradable { get; set; }
        public bool IsGradableType { get; set; }
        public bool WasSubmittedToAdmin { get; set; }

        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementReminder> AnnouncementReminders { get; set; }
        public IList<AnnouncementQnA> AnnouncementQnAs { get; set; } 
        public Person Owner { get; set; } 
    }


}
