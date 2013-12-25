using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AnnouncementInfo
    {
        public int AnnouncementId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int? ClassAnnouncementTypeId { get; set; }

        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool CanDropStudentScore { get; set; }
        public bool HideFromStudent { get; set; }

        public static AnnouncementInfo Create(Announcement announcement)
        {
            return new AnnouncementInfo
                {
                    ClassAnnouncementTypeId = announcement.ClassAnnouncementTypeRef,
                    AnnouncementId = announcement.Id,
                    Content = announcement.Content,
                    ExpiresDate = announcement.Expires,
                    Subject = announcement.Subject,
                    MaxScore = announcement.MaxScore,
                    WeightAddition = announcement.WeightAddition,
                    WeightMultiplier = announcement.WeightMultiplier,
                    CanDropStudentScore = announcement.MayBeDropped
                };
        }
    }
}
