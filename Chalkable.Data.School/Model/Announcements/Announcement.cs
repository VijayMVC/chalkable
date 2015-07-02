using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Announcements
{
    public enum AnnouncementState
    {
        Draft = 0,
        Created = 1
    }

    public enum AnnouncementType
    {
        None = 0,
        Class = 1,
        Admin = 2,
        LessonPlan = 3
    }

    public class Announcement
    {
        public const string ID_FIELD = "Id";
        public const string CONTENT_FIELD = "Content";
        public const string CREATED_FIELD = "Created";
        public const string STATE_FIELD = "State";
        public const string TITLE_FIELD = "Title";
        
        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public virtual int Id { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual AnnouncementState State { get; set; }
        public virtual string Title { get; set; }

        [NotDbFieldAttr]
        public bool IsOwner { get; set; }

        [NotDbFieldAttr]
        public virtual int OwnereId { get { return 0; } }

        [NotDbFieldAttr]
        public bool IsDraft
        {
            get { return State == AnnouncementState.Draft; }
        }
        
        [NotDbFieldAttr]
        public virtual bool IsSubmitted
        {
            get { return State == AnnouncementState.Created; }
        }

        [NotDbFieldAttr]
        public virtual string AnnouncementTypeName
        {
            get { return null; }
        }
        [NotDbFieldAttr]
        public virtual AnnouncementType Type
        {
            get { return AnnouncementType.None; }
        }
    }

    public class AnnouncementComplex : Announcement
    {
        public override bool IsSubmitted
        {
            get { return announcementData.IsSubmitted; }
        }
        public override AnnouncementType Type
        {
            get { return announcementData.Type; }
        }
        public override string AnnouncementTypeName
        {
            get { return announcementData.AnnouncementTypeName; }
        }
        public override int OwnereId
        {
            get { return announcementData.OwnereId; }
        }

        private Announcement announcementData;
        private LessonPlan lessonPlanData;
        private ClassAnnouncement classAnnouncementData;
        private AdminAnnouncement adminAnnouncementData;

        private int? classId;
        private int? adminId;

        public int? ClassRef { get { return classId; } }
        public int? AdminRef { get { return adminId; } }

        public Announcement AnnouncementData
        {
            set
            {
                announcementData = value;
                lessonPlanData = value as LessonPlan;
                classAnnouncementData = value as ClassAnnouncement;
                adminAnnouncementData = value as AdminAnnouncement;
                if (lessonPlanData != null)
                    classId = lessonPlanData.ClassRef;
                if (classAnnouncementData != null)
                    classId = classAnnouncementData.ClassRef;
                if (adminAnnouncementData != null)
                    adminId = adminAnnouncementData.AdminRef;
            }
        }
        
        public LessonPlan LessonPlanData { get { return lessonPlanData; } }
        public ClassAnnouncement ClassAnnouncementData { get { return classAnnouncementData; } }
        public AdminAnnouncement AdminAnnouncementData { get { return adminAnnouncementData; } }


        public bool Complete { get; set; }
        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int ApplicationCount { get; set; }

        public bool Gradable { get { return GradableType && ClassAnnouncementData.IsScored; } }
        public bool GradableType { get { return ClassAnnouncementData != null; } }


    }

    public class AnnouncementSummaryData
    {
        public int AnnouncementId { get; set; }
        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int ApplicationCount { get; set; }
    }

    public class AnnouncementDetails : AnnouncementComplex
    {
        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; }
        public Person Owner { get; set; }
        public IList<AnnouncementStandardDetails> AnnouncementStandards { get; set; }
        public IList<AnnouncementGroup> AnnouncementGroups { get; set; }
    }


}
