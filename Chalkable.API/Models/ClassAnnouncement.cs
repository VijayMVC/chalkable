using System;
using Newtonsoft.Json;

namespace Chalkable.API.Models
{
    public class ClassAnnouncement : ShortAnnouncement
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
    }
}
