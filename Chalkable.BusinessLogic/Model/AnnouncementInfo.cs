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
        public bool DiscussionEnabled { get; set; }
        public bool PreviewCommentsEnabled { get; set; }
        public bool RequireCommentsEnabled { get; set; }

        public DateTime? ExpiresDate { get; set; }
        public int? ClassAnnouncementTypeId { get; set; }

        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool CanDropStudentScore { get; set; }
        public bool HideFromStudents { get; set; }
        public bool Gradable { get; set; }


        public ClassAnnouncementInfo()
        {
            MaxScore = ClassAnnouncement.DEFAULT_MAX_SCORE;
            WeightAddition = ClassAnnouncement.DEFAULT_WEIGHT_ADDITION;
            WeightMultiplier = ClassAnnouncement.DEFAULT_WEGIHT_MULTIPLIER;
            Gradable = ClassAnnouncement.DEFAULT_IS_SCORED;
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
                    Title = announcement.Title,
                    Gradable = announcement.IsScored,
                    DiscussionEnabled = announcement.DiscussionEnabled,
                    PreviewCommentsEnabled = announcement.PreviewCommentsEnabled,
                    RequireCommentsEnabled = announcement.RequireCommentsEnabled
                };
        }
    }
}
