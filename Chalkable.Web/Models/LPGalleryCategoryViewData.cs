using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class LPGalleryCategoryViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public int LessonPlansCount { get; set; }

        
        public static LPGalleryCategoryViewData Create(LPGalleryCategory category)
        {
            return new LPGalleryCategoryViewData
                {
                    Id = category.Id,
                    Name = category.Name,
                    OwnerId = category.OwnerRef,
                    LessonPlansCount = category.LessonPlansCount
                };
        }

        public static IList<LPGalleryCategoryViewData> Create(IList<LPGalleryCategory> categories)
        {
            return categories.Select(Create).ToList();
        } 
    }
}