using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class SupplementalAnnouncementViewData : ShortAnnouncementViewData
    {
        public DateTime? ExpiresDate { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public bool HideFromStudents { get; set; }
        public IList<ShortPersonViewData> Recipients { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }

        protected SupplementalAnnouncementViewData(SupplementalAnnouncement announcement)
            : base(announcement)
        {
            AnnouncementTypeId = announcement.ClassAnnouncementTypeRef;
            ChalkableAnnouncementTypeId = announcement.ChalkableAnnouncementType;
            ExpiresDate = announcement.Expires;
            ClassId = announcement.ClassRef;
            ClassName = announcement.ClassName;
            FullClassName = announcement.FullClassName;
            HideFromStudents = !announcement.VisibleForStudent;
            PersonId = announcement.PrimaryTeacherRef;
            PersonName = announcement.PrimaryTeacherName;
            PersonGender = announcement.PrimaryTeacherGender;
            Recipients = announcement.Recipients?.Select(ShortPersonViewData.Create).ToList();
        }

        public static SupplementalAnnouncementViewData Create(SupplementalAnnouncement announcement)
        {
            return new SupplementalAnnouncementViewData(announcement);
        }
        public static IList<SupplementalAnnouncementViewData> Create(IList<SupplementalAnnouncement> announcements)
        {
            return announcements.Select(Create).ToList();
        }
    }
}