using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ActivityAssignedAttrToAnnouncementAssignedAttrMapper : BaseMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>
    {
        protected override void InnerMap(AnnouncementAssignedAttribute announcementAttribute, ActivityAssignedAttribute activityAttribute)
        {
            announcementAttribute.Name = activityAttribute.Name;
            announcementAttribute.VisibleForStudents = activityAttribute.VisibleInHomePortal;
            announcementAttribute.Text = activityAttribute.Text;
            announcementAttribute.SisActivityAssignedAttributeId = activityAttribute.Id;
            announcementAttribute.AttributeTypeId = activityAttribute.AttributeId;
            announcementAttribute.SisActivityId = activityAttribute.ActivityId;

            if (activityAttribute.Attachment != null)
            {
                if(announcementAttribute.SisAttributeAttachmentId != activityAttribute.Attachment.AttachmentId)
                    announcementAttribute.Uuid = activityAttribute.Attachment.CrocoDocId.HasValue
                        ? activityAttribute.Attachment.CrocoDocId.ToString()
                        : "";
                announcementAttribute.SisAttributeAttachmentId = activityAttribute.Attachment.AttachmentId;
                announcementAttribute.SisAttachmentMimeType = activityAttribute.Attachment.MimeType;
                announcementAttribute.SisAttachmentName = activityAttribute.Attachment.Name;
            }
        }
    }
}
