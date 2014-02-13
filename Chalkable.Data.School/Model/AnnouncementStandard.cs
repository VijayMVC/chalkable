using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementStandard
    {
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string STANDARD_REF_FIELD = "StandardRef";

        [PrimaryKeyFieldAttr]
        public int AnnouncementRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StandardRef { get; set; }
    }

    public class AnnouncementStandardDetails : AnnouncementStandard
    {
        [DataEntityAttr]
        public Standard Standard { get; set; }
    }
}
