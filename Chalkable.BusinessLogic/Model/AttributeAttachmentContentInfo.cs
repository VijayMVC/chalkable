using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class AttributeAttachmentContentInfo
    {
        public string AttachmentName { get; set; }
        public byte[] Content { get; set; }

        public static AttributeAttachmentContentInfo Create(string attachmentName, byte[] content)
        {
            return new AttributeAttachmentContentInfo { AttachmentName = attachmentName, Content = content };
        }
    }
}
