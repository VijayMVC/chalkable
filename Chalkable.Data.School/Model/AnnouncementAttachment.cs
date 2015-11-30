using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAttachment
    {
        public const string VW_ANNOUNCEMENT_ATTACHMENT = "vwAnnouncementAttachment";
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public DateTime AttachedDate { get; set; }
        public int Order { get; set; }
        public int AttachmentRef { get; set; }

        [DataEntityAttr]
        public Attachment Attachment { get; set; }
    }
}
