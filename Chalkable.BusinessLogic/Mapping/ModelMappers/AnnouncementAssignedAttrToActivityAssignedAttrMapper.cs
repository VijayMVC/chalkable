using System;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AnnouncementAssignedAttrToActivityAssignedAttrMapper : BaseMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>
    {
        protected override void InnerMap(ActivityAssignedAttribute activityAssignedAttribute, AnnouncementAssignedAttribute announcementAssignedAttribute)
        {
            if (announcementAssignedAttribute.SisActivityAssignedAttributeId.HasValue)
                activityAssignedAttribute.Id = announcementAssignedAttribute.SisActivityAssignedAttributeId.Value;
            
            activityAssignedAttribute.VisibleInHomePortal = announcementAssignedAttribute.VisibleForStudents;
            activityAssignedAttribute.Name = announcementAssignedAttribute.Name;
            activityAssignedAttribute.Text = announcementAssignedAttribute.Text;
            activityAssignedAttribute.AttributeId = (short)announcementAssignedAttribute.AttributeTypeId;

            if (announcementAssignedAttribute.SisAttributeAttachmentId.HasValue)
            {
                var stiAttachment = new StiAttachment
                {
                    AttachmentId = announcementAssignedAttribute.SisAttributeAttachmentId.Value,
                    CrocoDocId = !string.IsNullOrWhiteSpace(announcementAssignedAttribute.Uuid) ? new Guid(announcementAssignedAttribute.Uuid) : (Guid?)null,
                    Name = announcementAssignedAttribute.SisAttachmentName,
                    MimeType = announcementAssignedAttribute.SisAttachmentMimeType
                };

                activityAssignedAttribute.Attachment = stiAttachment;
            }
        }
    }
}
