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
                if (announcementAttribute.Attachment == null)
                    announcementAttribute.Attachment = new Attachment();
                MapperFactory.GetMapper<Attachment, StiAttachment>().Map(announcementAttribute.Attachment, activityAttribute.Attachment);
            }
            else
            {
                announcementAttribute.Attachment = null;
                announcementAttribute.AttachmentRef = null;
            }
        }
    }
}
