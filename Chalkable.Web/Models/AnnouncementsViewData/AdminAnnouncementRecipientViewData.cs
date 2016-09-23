using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AdminAnnouncementGroupViewData
    {
        public int AnnouncementId { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int StudentCount { get; set; }
        public IList<string> StudentsDisplayName { get; set; }

        public static AdminAnnouncementGroupViewData Create(AnnouncementGroup announcementRecipient)
        {
            return new AdminAnnouncementGroupViewData
            {
                AnnouncementId = announcementRecipient.AnnouncementRef,
                GroupId = announcementRecipient.GroupRef,
                GroupName = announcementRecipient.Group.Name,
                StudentCount = announcementRecipient.Students?.Count ?? 0,
                StudentsDisplayName = announcementRecipient.Students?.Select(x => x.DisplayName()).Take(30).ToList()
            };
        }

        public static IList<AdminAnnouncementGroupViewData> Create(IList<AnnouncementGroup> recipients)
        {
            return recipients.Select(Create).ToList();
        }

    }
}