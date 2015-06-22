using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class LPGalleryCategory
    {
        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerRef { get; set; }
    }
}
