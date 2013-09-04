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

        protected AnnouncementShortGradeViewData(AnnouncementComplex announcement, IGradingStyleMapper mapper)
            : base(announcement)
        {
            Avg = announcement.Avg;
            MappedAvg = mapper.Map(announcement.GradingStyle, announcement.Avg);
            GradedStudentCount = announcement.GradingsStudentsCount;
        }
        public static AnnouncementShortGradeViewData Create(AnnouncementComplex announcement, IGradingStyleMapper mapper)
        {
            return new AnnouncementShortGradeViewData(announcement, mapper);
        }
        public static IList<AnnouncementShortGradeViewData> Create(IList<AnnouncementComplex> announcements, IGradingStyleMapper mapper)
        {
            return announcements.Select(x => Create(x, mapper)).ToList();
        }
    }
}