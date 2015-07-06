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
            announcementAttribute.SisAttributeId = activityAttribute.Id;
            announcementAttribute.SisActivityId = activityAttribute.ActivityId;
            announcementAttribute.AttributeTypeId = activityAttribute.AttributeId;

            if (activityAttribute.Attachment != null)
            {
                announcementAttribute.Uuid = activityAttribute.Attachment.CrocoDocId.HasValue
                    ? activityAttribute.Attachment.CrocoDocId.ToString()
                    : "";
                announcementAttribute.SisAttributeAttachmentId = activityAttribute.Attachment.AttachmentId;
            }
        }
    }
}
