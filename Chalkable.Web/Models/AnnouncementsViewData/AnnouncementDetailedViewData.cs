using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementDetailedViewData : AnnouncementViewData
    {
        public IList<AnnouncementAttachmentViewData> AnnouncementAttachments { get; set; }
        public IList<AnnouncementAssignedAttributeViewData>  AnnouncementAttributes { get; set; }
        public IList<AnnouncementQnAViewData> AnnouncementQnAs { get; set; }
        public IList<AnnouncementApplicationViewData> Applications { get; set; }
        public IList<StandardViewData> Standards { get; set; }
        public StudentAnnouncementsViewData StudentAnnouncements { get; set; }
        public IList<AutoGradeViewData> AutoGradeApps { get; set; }
        public IList<StudentAnnouncementApplicationMetaViewData> StudentsAnnouncementApplicationMeta { get; set; }

        public ShortPersonViewData Owner { get; set; }
        public bool Exempt { get; set; }
         
        public bool CanRemoveStandard { get; set; }
        public bool IsAbleUseExtraCredit { get; set; }
        public bool IsAssessmentAttached { get; set; }

        public IList<BaseApplicationViewData> SuggestedApps { get; set; }
        public IList<BaseApplicationViewData> AppsWithContent { get; set; } 
        public IList<AdminAnnouncementGroupViewData> Recipients { get; set; }
        public IList<StudentViewData> AdminAnnouncementStudents { get; set; }
        public IList<AnnouncementCommentViewData> AnnouncementComments { get; set; }

        private AnnouncementDetailedViewData(AnnouncementDetails announcementDetails, IList<StudentAnnouncement> studentAnnouncements, IList<ClaimInfo> claims)
            : base(announcementDetails, studentAnnouncements, claims)
        {
            if (announcementDetails.AnnouncementQnAs != null)
                AnnouncementQnAs = AnnouncementQnAViewData.Create(announcementDetails.AnnouncementQnAs);


            if(announcementDetails.Owner != null)
                Owner = ShortPersonViewData.Create(announcementDetails.Owner);
            if(announcementDetails.AnnouncementStandards != null)
                Standards = StandardViewData.Create(announcementDetails.AnnouncementStandards);
            if (announcementDetails.AnnouncementApplications == null) return;
            Exempt = studentAnnouncements.Count > 0 && studentAnnouncements.All(x => x.Exempt);
            CanRemoveStandard = studentAnnouncements.Count == 0
                                || studentAnnouncements.All(x => string.IsNullOrEmpty(x.ScoreValue));
        }

        private AnnouncementDetailedViewData(AnnouncementComplex announcement, IList<ClaimInfo> claims)
            : base(announcement, claims)
        {
        }

        public static AnnouncementDetailedViewData CreateEmpty(AnnouncementComplex announcement, IList<ClaimInfo> claims)
        {
            return new AnnouncementDetailedViewData(announcement, claims);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, int currentSchoolPersonId, IList<ClaimInfo> claims)
        {
            var studentAnnouncements = announcementDetails.StudentAnnouncements.Select(x => new StudentAnnouncement
            {
                AnnouncementId = x.AnnouncementId,
                StudentId = x.StudentId,
                Comment = x.Comment,
                ScoreDropped = x.ScoreDropped,
                ExtraCredit = x.ExtraCredit,
                ScoreValue = x.ScoreValue,
                NumericScore = x.NumericScore,
                IsWithdrawn = x.Student.IsWithdrawn
            }).ToList();
            return new AnnouncementDetailedViewData(announcementDetails, studentAnnouncements, claims);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, int currentSchoolPersonId, 
            IList<AnnouncementAttachmentInfo> attachmentInfos, IList<AttachmentInfo> attrAttachmentInfos, IList<ClaimInfo> claims)
        {
            var res = Create(announcementDetails, currentSchoolPersonId, claims);
            res.AnnouncementAttachments = AnnouncementAttachmentViewData.Create(attachmentInfos, currentSchoolPersonId);

            if (announcementDetails.AnnouncementAttributes != null)
                res.AnnouncementAttributes = AnnouncementAssignedAttributeViewData.Create(announcementDetails.AnnouncementAttributes, attrAttachmentInfos);
            return res;
        }
    }
}