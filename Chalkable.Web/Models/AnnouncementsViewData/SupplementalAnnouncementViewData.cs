using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class SupplementalAnnouncementViewData : ShortAnnouncementViewData
    {
        public DateTime? ExpiresDate { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public bool HideFromStudents { get; set; }

        protected SupplementalAnnouncementViewData(SupplementalAnnouncement announcement)
            : base(announcement)
        {
            ExpiresDate = announcement.Expires;
            ClassId = announcement.ClassRef;
            ClassName = announcement.ClassName;
            FullClassName = announcement.FullClassName;
            HideFromStudents = !announcement.VisibleForStudent;
            PersonId = announcement.PrimaryTeacherRef;
            PersonName = announcement.PrimaryTeacherName;
            PersonGender = announcement.PrimaryTeacherGender;
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