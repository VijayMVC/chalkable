using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.ApplicationsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementDetailedViewData : AnnouncementGradeViewData
    {
        public IList<AnnouncementAttachmentViewData> AnnouncementAttachments { get; set; }
        public IList<AnnouncementQnAViewData> AnnouncementQnAs { get; set; }
        public IList<AnnouncementReminderViewData> AnnouncementReminders { get; set; }
        public IList<AnnouncementApplicationViewData> Applications { get; set; }
        public IList<AnnouncementStandardViewData> Standards { get; set; }
        public StudentAnnouncementsViewData StudentAnnouncements { get; set; }
        public IList<String> autoGradeApps { get; set; }

        public ShortPersonViewData Owner { get; set; }
        
        private AnnouncementDetailedViewData(AnnouncementDetails announcementDetails, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, int currentSchoolPersonId)
            : base(announcementDetails, studentAnnouncements, mapper, announcementDetails.Gradable, null)
        {
            if (announcementDetails.AnnouncementQnAs != null)
                AnnouncementQnAs = AnnouncementQnAViewData.Create(announcementDetails.AnnouncementQnAs);

            Owner = ShortPersonViewData.Create(announcementDetails.Owner);
            AnnouncementReminders = AnnouncementReminderViewData.Create(announcementDetails.AnnouncementReminders, currentSchoolPersonId, Owner.Id);
            if(announcementDetails.AnnouncementStandards != null)
                Standards = AnnouncementStandardViewData.Create(announcementDetails.AnnouncementStandards);
            if (announcementDetails.AnnouncementApplications == null) return;
            //TODO: applicationViewData
            //Applications = new List<AnnouncementApplicationViewData>();
            //foreach (var announcementApplication in announcementDetails.AnnouncementApplications)
            //{
            //    Applications.Add(AnnouncementApplicationViewData.Create(announcementApplication, currentSchoolPersonId));
            //}
        }

        private AnnouncementDetailedViewData(AnnouncementComplex announcement, bool isGradable, bool? wasAnnouncementTypeGraded)
            : base(announcement, isGradable, wasAnnouncementTypeGraded)
        {
        }

        public static AnnouncementDetailedViewData CreateEmpty(AnnouncementComplex announcement, bool isGradable = false, bool? wasAnnouncementTypeGraded = null)
        {
            return new AnnouncementDetailedViewData(announcement, isGradable, wasAnnouncementTypeGraded);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, IGradingStyleMapper mapper, int currentSchoolPersonId)
        {
            var studentAnnouncements = announcementDetails.StudentAnnouncements.Select(x => new StudentAnnouncement
            {
                Id = x.Id,
                AnnouncementRef = x.AnnouncementRef,
                PersonRef = x.PersonRef,
                Comment = x.Comment,
                Dropped = x.Dropped,
                ExtraCredit = x.ExtraCredit,
                GradeValue = x.GradeValue
            }).ToList();
            return new AnnouncementDetailedViewData(announcementDetails, studentAnnouncements, mapper, currentSchoolPersonId);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, IGradingStyleMapper mapper,
            int currentSchoolPersonId, IList<AnnouncementAttachmentInfo> attachmentInfos)
        {
            var res = Create(announcementDetails, mapper, currentSchoolPersonId);
            res.AnnouncementAttachments = AnnouncementAttachmentViewData.Create(attachmentInfos, currentSchoolPersonId);
            return res;
        }
    }
}