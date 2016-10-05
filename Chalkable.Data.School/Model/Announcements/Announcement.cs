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

    public enum AnnouncementTypeEnum
    {
        None = 0,
        Class = 1,
        Admin = 2,
        LessonPlan = 3,
        Supplemental = 4
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
        public virtual bool DiscussionEnabled { get; set; }
        public virtual bool PreviewCommentsEnabled { get; set; }
        public virtual bool RequireCommentsEnabled { get; set; }
        [NotDbFieldAttr]
        public bool IsOwner { get; set; }
        [NotDbFieldAttr]
        public virtual int? OwnereId => 0;
        [NotDbFieldAttr]
        public bool IsDraft => State == AnnouncementState.Draft;
        [NotDbFieldAttr]
        public virtual bool IsSubmitted => State == AnnouncementState.Created;
        [NotDbFieldAttr]
        public virtual string AnnouncementTypeName => null;
        [NotDbFieldAttr]
        public virtual AnnouncementTypeEnum Type => AnnouncementTypeEnum.None;
    }

    public class AnnouncementComplex : Announcement
    {
        public override bool IsSubmitted => _announcementData.IsSubmitted;
        public override AnnouncementTypeEnum Type => _announcementData.Type;
        public override string AnnouncementTypeName => _announcementData.AnnouncementTypeName;
        public override int? OwnereId => _announcementData.OwnereId;

        private Announcement _announcementData;

        private int? classId;
        private int? adminId;
        public int? ClassRef => classId;
        public int? AdminRef => adminId;

        public Announcement AnnouncementData
        {
            set
            {
                _announcementData = value;
                LessonPlanData = value as LessonPlan;
                ClassAnnouncementData = value as ClassAnnouncement;
                AdminAnnouncementData = value as AdminAnnouncement;
                SupplementalAnnouncementData = value as SupplementalAnnouncement;

                if (LessonPlanData != null)
                    classId = LessonPlanData.ClassRef;
                if (ClassAnnouncementData != null)
                    classId = ClassAnnouncementData.ClassRef;
                if (SupplementalAnnouncementData != null)
                    classId = SupplementalAnnouncementData.ClassRef;
                if (AdminAnnouncementData != null)
                    adminId = AdminAnnouncementData.AdminRef;
            }
            get { return _announcementData;}
        }

        public LessonPlan LessonPlanData { get; private set; }
        public ClassAnnouncement ClassAnnouncementData { get; private set; }
        public AdminAnnouncement AdminAnnouncementData { get; private set; }
        public SupplementalAnnouncement SupplementalAnnouncementData { get; private set; }

        public bool Complete { get; set; }
        public int QnACount { get; set; }
        public int StudentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int ApplicationCount { get; set; }
        public int StandardsCount { get; set; }
        public IList<string> AttachmentNames { get; set; }
        public bool Gradable => GradableType && ClassAnnouncementData.IsScored;
        public bool GradableType => ClassAnnouncementData != null;
    }

    public class AnnouncementDetails : AnnouncementComplex
    {
        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }
        public IList<AnnouncementApplication> AnnouncementApplications { get; set; }
        public IList<AnnouncementAttachment> AnnouncementAttachments { get; set; }
        public IList<AnnouncementQnAComplex> AnnouncementQnAs { get; set; }
        public IList<AnnouncementAssignedAttribute> AnnouncementAttributes { get; set; }
        public Person Owner { get; set; }
        public IList<AnnouncementStandardDetails> AnnouncementStandards { get; set; }
        public IList<AnnouncementGroup> AnnouncementGroups { get; set; }
        public IList<AdminAnnouncementStudent> AdminAnnouncementStudents { get; set; }
    }
    
}
