using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AttachmentContentInfo
    {
        public AnnouncementAttachment Attachment { get; set; }
        public byte[] Content { get; set; }

        public static AttachmentContentInfo Create(AnnouncementAttachment attachment, byte[] content)
        {
            return new AttachmentContentInfo {Attachment = attachment, Content = content};
        }
    }
}
