using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AdminAnnouncementGroupViewData
    {
        public int AnnouncementId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public static AdminAnnouncementGroupViewData Create(AnnouncementGroup announcementRecipient)
        {
            return new AdminAnnouncementGroupViewData
            {
                AnnouncementId = announcementRecipient.AnnouncementRef,
                GroupId = announcementRecipient.GroupRef,
                GroupName = announcementRecipient.Group.Name
            };
        }

        public static IList<AdminAnnouncementGroupViewData> Create(IList<AnnouncementGroup> recipients)
        {
            return recipients.Select(Create).ToList();
        }

    }
}