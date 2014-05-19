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
    public class Announcement
    {
        public const string ID_FIELD = "Id";
        public const string CONTENT_FIELD = "Content";
        public const string CREATED_FIELD = "Created";
        public const string STATE_FIELD = "State";
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string CLASS_ANNOUNCEMENT_TYPE_REF_FIELD = "ClassAnnouncementTypeRef";
        public const string TITLE_FIELD = "Title";
        public const string SIS_ACTIVITY_ID_FIELD = "SisActivityId";

        public const string PRIMARY_TEACHER_REF_FIELD = "PrimaryTeacherRef";
        
        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public int? ClassAnnouncementTypeRef { get; set; }
        public AnnouncementState State { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
        public string Subject { get; set; }
        public int ClassRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }

        public int? SisActivityId { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool MayBeDropped { get; set; }
        [NotDbFieldAttr]
        public bool MayBeExempt { get; set; }
        [NotDbFieldAttr]
        public bool IsScored { get; set; }
        public bool VisibleForStudent { get; set; }

        public int SchoolRef { get; set; }

        public virtual string Title { get; set; }

        [NotDbFieldAttr]
        public int PrimaryTeacherRef { get; set; }
        [NotDbFieldAttr]
        public bool IsOwner { get; set; }
       
    }


    public class AnnouncementComplex : Announcement
    {
        public string ClassAnnouncementTypeName { get; set; }
        public int? ChalkableAnnouncementType { get; set; }
        public string PrimaryTeacherGender { get; set; }
        public string PrimaryTeacherName { get; set; }
   
        public string ClassName { get; set; }
        public int? GradeLevelId { get; set; }
        
        
        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int? Avg { get; set; }
        public int ApplicationCount { get; set; }

        public int? RecipientDataPersonId { get; set; }
        public bool? Starred { get; set; }
        
        public bool IsDraft
        {
            get { return State == AnnouncementState.Draft; }
        }

        public override string Title
        {
            get { return !string.IsNullOrEmpty(base.Title) ? base.Title : DefaultTitle;}
            set { base.Title = value; }
        }

        public string DefaultTitle
        {
            get { return string.Format("{0} {1}", ClassAnnouncementTypeName, Order); }
        }

        public bool Gradable
        {
            get { return GradableType && IsScored; }
        }
        public bool GradableType  
        {
            get { return true; }
        }
      
    }
    public class AnnouncementDetails : AnnouncementComplex
    {   
        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementReminder> AnnouncementReminders { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; } 
        public Person Owner { get; set; }
        public IList<AnnouncementStandardDetails> AnnouncementStandards { get; set; } 
    }


}
