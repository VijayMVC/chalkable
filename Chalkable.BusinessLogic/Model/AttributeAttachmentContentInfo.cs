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

    public class AnnouncementAssignedAttributeInfo
    {
        public AnnouncementAssignedAttribute Attribute { get; set; }
        public AttributeAttachmentContentInfo AttachmentContentInfo { get; set; }

        public static AnnouncementAssignedAttributeInfo Create(AnnouncementAssignedAttribute attribute,
                                                               AttributeAttachmentContentInfo attachmentContentInfo)
        {
            return new AnnouncementAssignedAttributeInfo
                {
                    AttachmentContentInfo = attachmentContentInfo,
                    Attribute = attribute
                };
        }
    }
}
