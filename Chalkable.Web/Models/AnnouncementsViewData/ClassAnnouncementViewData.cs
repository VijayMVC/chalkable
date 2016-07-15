using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class ClassAnnouncementViewData : ShortAnnouncementViewData
    {
        public DateTime? ExpiresDate { get; set; }
        public string DefaultTitle { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool Dropped { get; set; }
        public int Order { get; set; }
        public decimal? MaxScore { get; set; }
        public bool CanDropStudentScore { get; set; }
        public bool MayBeExempt { get; set; }
        public bool Gradable { get; set; }
        public bool CanGrade { get; set; }
        public bool HideFromStudents { get; set; }

        public decimal? WeightMultiplier { get; set; }
        public decimal? WeightAddition { get; set; }

        protected ClassAnnouncementViewData(ClassAnnouncement announcement, IList<ClaimInfo> claims)
            : base(announcement)
        {
            DefaultTitle = announcement.DefaultTitle;
            AnnouncementTypeId = announcement.ClassAnnouncementTypeRef;
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
            MaxScore = announcement.MaxScore;
            CanDropStudentScore = announcement.MayBeDropped;
            MayBeExempt = announcement.MayBeExempt;
            Gradable = announcement.IsScored;
            CanGrade = (IsOwner || claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN)) && announcement.IsScored;
            DepartmentId = announcement.DepartmentId;
            HideFromStudents = !announcement.VisibleForStudent;
            WeightAddition = announcement.WeightAddition;
            WeightMultiplier = announcement.WeightMultiplier;
        }

        public static ClassAnnouncementViewData Create(ClassAnnouncement announcement, IList<ClaimInfo> claims)
        {
            return new ClassAnnouncementViewData(announcement, claims);
        }
        public static IList<ClassAnnouncementViewData> Create(IList<ClassAnnouncement> announcements, IList<ClaimInfo> claims)
        {
            return announcements.Select(x => Create(x, claims)).ToList();
        }
    }

}