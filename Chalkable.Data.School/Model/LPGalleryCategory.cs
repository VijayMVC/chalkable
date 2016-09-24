using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class LPGalleryCategory
    {
        public const string ID_FIELD = "Id";
        public const string NAME_FIELD = "Name";

        public const string VW_LP_GALLERY_CATEGORY = "vwLPGalleryCategory";

        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerRef { get; set; }

        [NotDbFieldAttr]
        public int LessonPlansCount { get; set; }
    }
}
