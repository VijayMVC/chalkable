using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AdminAnnouncementRecipientViewData
    {
        public int AnnouncementId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public static AdminAnnouncementRecipientViewData Create(AdminAnnouncementRecipient announcementRecipient)
        {
            return new AdminAnnouncementRecipientViewData
            {
                AnnouncementId = announcementRecipient.Id,
                GroupId = announcementRecipient.GroupRef,
                GroupName = announcementRecipient.Group.Name
            };
        }

        public static IList<AdminAnnouncementRecipientViewData> Create(IList<AdminAnnouncementRecipient> recipients)
        {
            return recipients.Select(Create).ToList();
        }

    }
}