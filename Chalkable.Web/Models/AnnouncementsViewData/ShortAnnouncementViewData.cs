using System;
using System.Collections.Generic;
using System.Linq;
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

        protected AnnouncementShortGradeViewData(ClassAnnouncement ann)
            : base(ann)
        {

        }
        public static AnnouncementShortGradeViewData Create(AnnouncementComplex announcementComplex, int? studentGrade = null)
        {
            var annData = announcementComplex.ClassAnnouncementData;
            return new AnnouncementShortGradeViewData(annData)
                {
                    GradedStudentCount = announcementComplex.GradingStudentsCount,
                    Grade = studentGrade
                };
        }
        public static IList<AnnouncementShortGradeViewData> Create(IList<AnnouncementComplex> announcements)
        {
            return announcements.Select(x => Create(x)).ToList();
        }
    }
}