using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class ShortAnnouncementViewData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AnnouncementTypeName { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public DateTime? Created { get; set; }
        public int State { get; set; }

        public bool IsOwner { get; set; }
        
        public int? PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
        public bool DiscussionEnabled { get; set; }
        public bool PreviewCommentsEnabled { get; set; }
        public bool RequireCommentsEnabled { get; set; }

        protected ShortAnnouncementViewData(){}

        protected ShortAnnouncementViewData(Announcement announcement)
        {
            Id = announcement.Id;
            Title = announcement.Title;
            Content = announcement.Content;
            Created = announcement.Created;
            State = (int) announcement.State;
            AnnouncementTypeName = announcement.AnnouncementTypeName;
            Type = (int)announcement.Type;
            IsOwner = announcement.IsOwner;
            DiscussionEnabled = announcement.DiscussionEnabled;
            PreviewCommentsEnabled = announcement.PreviewCommentsEnabled;
            RequireCommentsEnabled = announcement.RequireCommentsEnabled;
        } 

        public static ShortAnnouncementViewData Create(Announcement announcement)
        {
            return new ShortAnnouncementViewData(announcement);
        }
    }

    public class AnnouncementShortGradeViewData : ClassAnnouncementViewData
    {
        public int? Avg { get; set; }
        public int? MappedAvg { get; set; }
        public int GradedStudentCount { get; set; }
        public int? Grade { get; set; } //TODO: think about this

        protected AnnouncementShortGradeViewData(ClassAnnouncement ann, IList<ClaimInfo> claims)
            : base(ann, claims)
        {

        }
        public static AnnouncementShortGradeViewData Create(AnnouncementComplex announcementComplex, IList<ClaimInfo> claims, int? studentGrade = null)
        {
            var annData = announcementComplex.ClassAnnouncementData;
            return new AnnouncementShortGradeViewData(annData, claims)
                {
                    GradedStudentCount = announcementComplex.GradingStudentsCount,
                    Grade = studentGrade
                };
        }
        public static IList<AnnouncementShortGradeViewData> Create(IList<AnnouncementComplex> announcements, IList<ClaimInfo> claims)
        {
            return announcements.Select(x => Create(x, claims)).ToList();
        }
    }
}