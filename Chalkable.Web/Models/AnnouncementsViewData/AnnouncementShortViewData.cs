using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementShortViewData
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AnnouncementTypeName { get; set; }
        public int AnnouncementTypeId { get; set; }
        public Guid PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? ClassId { get; set; }
        public string ClassName { get; set; }
        public bool Dropped { get; set; }
        public bool IsOwner { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int Order { get; set; }
        public string Subject { get; set; }

        protected AnnouncementShortViewData(AnnouncementComplex announcement)
        {
            Id = announcement.Id;
            Title = announcement.Title;
            AnnouncementTypeId = announcement.AnnouncementTypeRef;
            AnnouncementTypeName = announcement.AnnouncementTypeName;
            PersonId = announcement.PersonRef;
            PersonName = announcement.PersonName;
            PersonGender = announcement.Gender;
            ClassId = announcement.ClassId;
            ClassName = announcement.ClassName;
            CourseId = announcement.CourseId;
            Dropped = announcement.Dropped;
            ExpiresDate = announcement.Expires;
            Order = announcement.Order;
            IsOwner = announcement.IsOwner;
            Subject = announcement.Subject;
        }

        public static AnnouncementShortViewData Create(AnnouncementComplex announcement)
        {
            return new AnnouncementShortViewData(announcement);
        }
        public static IList<AnnouncementShortViewData> Create(IList<AnnouncementComplex> announcements)
        {
            return announcements.Select(Create).ToList();
        } 
    }


    public class AnnouncementShortGradeViewData : AnnouncementShortViewData
    {
        public int? Avg { get; set; }
        public int? MappedAvg { get; set; }
        public int GradedStudentCount { get; set; }
        public int? Grade { get; set; } //TODO: think about this

        protected AnnouncementShortGradeViewData(AnnouncementComplex announcement, IGradingStyleMapper mapper)
            : base(announcement)
        {
            Avg = announcement.Avg;
            MappedAvg = mapper.Map(announcement.GradingStyle, announcement.Avg);
            GradedStudentCount = announcement.GradingsStudentsCount;
        }
        public static AnnouncementShortGradeViewData Create(AnnouncementComplex announcement, IGradingStyleMapper mapper, int? studentGrade = null)
        {
            return new AnnouncementShortGradeViewData(announcement, mapper) {Grade = studentGrade};
        }
        public static IList<AnnouncementShortGradeViewData> Create(IList<AnnouncementComplex> announcements, IGradingStyleMapper mapper)
        {
            return announcements.Select(x => Create(x, mapper)).ToList();
        }
    }
}