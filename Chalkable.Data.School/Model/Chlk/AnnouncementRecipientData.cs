using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Chlk
{
    public class AnnouncementRecipientData
    {
        [PrimaryKeyFieldAttr]
        public int AnnouncementRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        public bool Complete { get; set; }
    }
}
