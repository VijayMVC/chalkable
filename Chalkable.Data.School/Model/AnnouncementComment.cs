using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementComment
    {
        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int? AttachmentRef { get; set; }
        public int PersonRef { get; set; }
        public int? ParentCommentRef { get; set; }
        public string Text { get; set; }
        public DateTime PostedDate { get; set; }
        public bool Hiddent { get; set; }
        public bool Deleted { get; set; }

        [NotDbFieldAttr]
        public AnnouncementComment ParentComment { get; set; }
        [NotDbFieldAttr]
        public Attachment Attachment { get; set; }
        [NotDbFieldAttr]
        public Person Person { get; set; }
    }
}
