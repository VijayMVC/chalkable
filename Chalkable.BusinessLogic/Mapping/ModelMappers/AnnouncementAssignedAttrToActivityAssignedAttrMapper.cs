using System;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AnnouncementAssignedAttrToActivityAssignedAttrMapper : BaseMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>
    {
        protected override void InnerMap(ActivityAssignedAttribute activityAssignedAttribute, AnnouncementAssignedAttribute announcementAssignedAttribute)
        {
            if (!announcementAssignedAttribute.SisAttributeId.HasValue ||
                !announcementAssignedAttribute.SisActivityId.HasValue) return;

            activityAssignedAttribute.Id = announcementAssignedAttribute.SisAttributeId.Value;
            activityAssignedAttribute.Id = announcementAssignedAttribute.SisAttributeId.Value;
            activityAssignedAttribute.VisibleInHomePortal = announcementAssignedAttribute.VisibleForStudents;
            activityAssignedAttribute.Name = announcementAssignedAttribute.Name;
            activityAssignedAttribute.Text = announcementAssignedAttribute.Text;
            activityAssignedAttribute.AttributeId = (short)announcementAssignedAttribute.AttributeTypeId;

            if (announcementAssignedAttribute.SisAttributeAttachmentId.HasValue)
            {
                var stiAttachment = new StiAttachment
                {
                    AttachmentId = announcementAssignedAttribute.SisAttributeAttachmentId.Value,
                    CrocoDocId = new Guid(announcementAssignedAttribute.Uuid),
                    Name = announcementAssignedAttribute.SisAttachmentName,
                    MimeType = announcementAssignedAttribute.SisAttachmentMimeType
                };

                activityAssignedAttribute.Attachment = stiAttachment;
            }
        }
    }
}
