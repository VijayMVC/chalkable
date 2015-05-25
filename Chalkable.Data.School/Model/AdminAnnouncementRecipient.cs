using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AdminAnnouncementRecipient
    {
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string GROUP_REF_FIELD = "GroupRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int GroupRef { get; set; }

        [DataEntityAttr]
        public Group Group { get; set; }
    }
}
