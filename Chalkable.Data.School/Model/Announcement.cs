﻿using System;
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
        public Guid Id { get; set; }
        public const string PERSON_REF_FIELD = "PersonRef";
        public Guid PersonRef { get; set; }
        public const string CONTENT_FIELD = "Content";
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public int AnnouncementTypeRef { get; set; }
        public AnnouncementState State { get; set; }
        public GradingStyleEnum GradingStyle { get; set; }
        public string Subject { get; set; }
        public Guid? MarkingPeriodClassRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }
    }


    public class AnnouncementComplex : Announcement
    {
        public string AnnouncementTypeName { get; set; }
        public string PersonName { get; set; }
        public string Gender { get; set; }

        public Guid? ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid? GradeLevelId { get; set; }
        public Guid? CourseId { get; set; }

        public Guid? MarkingPeriodId { get; set; }

        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingsStudentsCount { get; set; }
        public int? Avg { get; set; }
        public int ApplicationCount { get; set; }

        public bool IsOwner { get; set; }
        public Guid? RecipientDataPersonId { get; set; }
        public bool? Starred { get; set; }
        
        public bool IsDraft
        {
            get { return State == AnnouncementState.Draft; }
        }

        public string Title
        {
            get
            {
                if (AnnouncementTypeRef == (int)SystemAnnouncementType.Admin)
                {
                    return Subject;
                }
                return !string.IsNullOrEmpty(ClassName) ? ClassName : "All";
            }
        }

        public bool Gradable
        {
            get { return GradableType && IsOwner; }
        }
        public bool GradableType  
        {
            get
            {
               var systemType = new AnnouncementType {Id = AnnouncementTypeRef, Name = AnnouncementTypeName}.SystemType;
               return (systemType == SystemAnnouncementType.Essay
                    || systemType == SystemAnnouncementType.Final
                    || systemType == SystemAnnouncementType.Test
                    || systemType == SystemAnnouncementType.Quiz
                    || systemType == SystemAnnouncementType.Project
                    || systemType == SystemAnnouncementType.HW
                    || systemType == SystemAnnouncementType.TermPaper
                    || systemType == SystemAnnouncementType.BookReport
                    || systemType == SystemAnnouncementType.Midterm);
            }
        }
      
    }

    public class AnnouncementDetails : AnnouncementComplex
    {
        public int? FinalGradeStatus { get; set; }
        
        public FinalGradeStatus? FinalGradeStatusTyped
        {
            get { return (FinalGradeStatus?) FinalGradeStatus; }
        }

        public bool WasSubmittedToAdmin
        {
            get { return FinalGradeStatusTyped.HasValue && FinalGradeStatusTyped.Value == Model.FinalGradeStatus.Submit; }
        }

        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementReminder> AnnouncementReminders { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; } 
        public Person Owner { get; set; } 
    }


}
