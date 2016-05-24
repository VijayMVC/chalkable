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

            if (announcementAssignedAttribute.Attachment != null)
            {
                if(activityAssignedAttribute.Attachment == null)
                    activityAssignedAttribute.Attachment = new StiAttachment();
                MapperFactory.GetMapper<StiAttachment, Attachment>().Map(activityAssignedAttribute.Attachment, announcementAssignedAttribute.Attachment);
            }
        }
    }
}
