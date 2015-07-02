using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AdminAnnouncementViewData : ShortAnnouncementViewData
    {
        public DateTime? ExpiresData { get; set; }

        protected AdminAnnouncementViewData(AdminAnnouncement announcement)
            : base(announcement)
        {
            PersonId = announcement.AdminRef;
            PersonName = announcement.AdminName;
            PersonGender = announcement.AdminGender;
            ExpiresData = announcement.Expires;
        }

        public static AdminAnnouncementViewData Create(AdminAnnouncement announcement)
        {
            return new AdminAnnouncementViewData(announcement);
        }

        public static IList<AdminAnnouncementViewData> Create(IList<AdminAnnouncement> adminAnnouncements)
        {
            return adminAnnouncements.Select(Create).ToList();
        }
    }
}