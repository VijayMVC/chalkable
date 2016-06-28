using System;

namespace Chalkable.API.Models
{
    public class LessonPlan : ShortAnnouncement
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string FullClassName { get; set; }
        public bool HideFromStudents { get; set; }
        public int? LpGalleryCategoryId { get; set; }
        public int? GalleryOwnerRef { get; set; }
        public bool InGallery { get; set; }
    }
}
