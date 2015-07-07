using System;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Model
{
    public class  ClassAnnouncementInfo
    {
        public int AnnouncementId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int? ClassAnnouncementTypeId { get; set; }

        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool CanDropStudentScore { get; set; }
        public bool HideFromStudents { get; set; }


        public ClassAnnouncementInfo()
        {
            MaxScore = 100;
            WeightAddition = 0;
            WeightMultiplier = 1;
        }

        public static ClassAnnouncementInfo Create(ClassAnnouncement announcement)
        {
            return new ClassAnnouncementInfo
                {
                    ClassAnnouncementTypeId = announcement.ClassAnnouncementTypeRef,
                    AnnouncementId = announcement.Id,
                    Content = announcement.Content,
                    ExpiresDate = announcement.Expires,
                    MaxScore = announcement.MaxScore,
                    WeightAddition = announcement.WeightAddition,
                    WeightMultiplier = announcement.WeightMultiplier,
                    CanDropStudentScore = announcement.MayBeDropped,
                    HideFromStudents = !announcement.VisibleForStudent,
                    Title = announcement.Title
                };
        }
    }
}
