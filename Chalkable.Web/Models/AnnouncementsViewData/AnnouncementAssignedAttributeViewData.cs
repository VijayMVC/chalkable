using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementAssignedAttributeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AttributeTypeId { get; set; }
        public int AnnouncementRef { get; set; }
        public string Uuid { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }
        public int? SisActivityAssignedAttributeId { get; set; }
        public AttachmentViewData AttributeAttachment { get; set; }

        public static AnnouncementAssignedAttributeViewData Create(AnnouncementAssignedAttribute attr, AttachmentInfo attachmentInfo)
        {
            var result = new AnnouncementAssignedAttributeViewData
                {
                    Id = attr.Id,
                    Name = attr.Name,
                    Text = attr.Text,
                    AttributeTypeId = attr.AttributeTypeId,
                    VisibleForStudents = attr.VisibleForStudents,
                    AnnouncementRef = attr.AnnouncementRef,
                    SisActivityAssignedAttributeId = attr.SisActivityAssignedAttributeId,
                    AttributeAttachment = attachmentInfo != null ? AttachmentViewData.Create(attachmentInfo) : null
                };
            return result;
        }

        public static IList<AnnouncementAssignedAttributeViewData> Create(IList<AnnouncementAssignedAttribute> announcementAttributes, IList<AttachmentInfo> attrAttachmentInfos)
        {
            return announcementAttributes.Select(annAtrr => Create(annAtrr, attrAttachmentInfos.FirstOrDefault(x=>x.Attachment.Id == annAtrr.AttachmentRef))).ToList();
        }
    }
}