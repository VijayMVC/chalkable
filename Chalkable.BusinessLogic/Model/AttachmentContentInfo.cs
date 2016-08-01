using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AttachmentContentInfo
    {
        public Attachment Attachment { get; set; }
        public byte[] Content { get; set; }

        public static AttachmentContentInfo Create(Attachment attachment, byte[] content)
        {
            return new AttachmentContentInfo {Attachment = attachment, Content = content};
        }
    }
}
