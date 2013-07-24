using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AnnouncementInfo
    {
        public Guid AnnouncementId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int AnnouncementTypeId { get; set; }

        public static AnnouncementInfo Create(Announcement announcement)
        {
            return new AnnouncementInfo
                {
                    AnnouncementTypeId = announcement.AnnouncementTypeRef,
                    AnnouncementId = announcement.Id,
                    Content = announcement.Content,
                    ExpiresDate = announcement.Expires,
                    Subject = announcement.Subject
                };
        }
    }
}
