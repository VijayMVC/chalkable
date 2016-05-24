using System;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Data.School.Model.Announcements
{
    public class InputClassAnnouncement 
    {
        public virtual int Id { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual AnnouncementState State { get; set; }
        public virtual string Title { get; set; }
        public DateTime Expires { get; set; }
        public int? ClassAnnouncementTypeRef { get; set; }
        public int ClassRef { get; set; }
        public int SchoolYearRef { get; set; }
        public int Order { get; set; }
        public bool Dropped { get; set; }
        public int? SisActivityId { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool VisibleForStudent { get; set; }
        public bool MayBeDropped { get; set; }
        public bool IsScored { get; set; }


        public static InputClassAnnouncement Create(ClassAnnouncement classAnnouncement)
        {
            return new InputClassAnnouncement
            {
                Id = classAnnouncement.Id,
                Content = classAnnouncement.Content,
                Created = classAnnouncement.Created,
                State = classAnnouncement.State,
                Title = classAnnouncement.Title,
                Expires = classAnnouncement.Expires,
                ClassAnnouncementTypeRef = classAnnouncement.ClassAnnouncementTypeRef,
                ClassRef = classAnnouncement.ClassRef,
                SchoolYearRef = classAnnouncement.SchoolYearRef,
                Order = classAnnouncement.Order,
                Dropped = classAnnouncement.Dropped,
                SisActivityId = classAnnouncement.SisActivityId,
                MaxScore = classAnnouncement.MaxScore,
                WeightAddition = classAnnouncement.WeightAddition,
                WeightMultiplier = classAnnouncement.WeightMultiplier,
                VisibleForStudent = classAnnouncement.VisibleForStudent,
                MayBeDropped = classAnnouncement.MayBeDropped,
                IsScored = classAnnouncement.IsScored
            };
        }

        public static IList<InputClassAnnouncement> Create(IList<ClassAnnouncement> classAnnouncements)
        {
            return classAnnouncements.Select(Create).ToList();
        } 
    }
}