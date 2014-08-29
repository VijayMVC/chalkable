using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementShortViewData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DefaultTitle { get; set; }
        public string AnnouncementTypeName { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool Dropped { get; set; }
        public bool IsOwner { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public int Order { get; set; }
        public string Subject { get; set; }
        public decimal? MaxScore { get; set; }
        public bool CanDropStudentScore { get; set; }
        public bool MayBeExempt { get; set; }
        public bool Gradable { get; set; }
        public bool CanGrade { get; set; }

        //todo add property departmentid 

        protected AnnouncementShortViewData(AnnouncementComplex announcement)
        {
            Id = announcement.Id;
            DefaultTitle = announcement.DefaultTitle;
            Title = announcement.Title;
            AnnouncementTypeId = announcement.ClassAnnouncementTypeRef;
            AnnouncementTypeName = announcement.ClassAnnouncementTypeName;
            ChalkableAnnouncementTypeId = announcement.ChalkableAnnouncementType;
            PersonId = announcement.PrimaryTeacherRef;
            PersonName = announcement.PrimaryTeacherName;
            PersonGender = announcement.PrimaryTeacherGender;
            ClassId = announcement.ClassRef;
            ClassName = announcement.ClassName;
            FullClassName = announcement.FullClassName;
            Dropped = announcement.Dropped;
            ExpiresDate = announcement.Expires == DateTime.MinValue ? (DateTime?)null : announcement.Expires;
            Order = announcement.Order;
            IsOwner = announcement.IsOwner;
            Subject = announcement.Subject;
            MaxScore = announcement.MaxScore;
            CanDropStudentScore = announcement.MayBeDropped;
            MayBeExempt = announcement.MayBeExempt;
            Gradable = announcement.Gradable;
            CanGrade = Gradable && IsOwner;
            DepartmentId = announcement.DepartmentId;
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
            GradedStudentCount = announcement.GradingStudentsCount;
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