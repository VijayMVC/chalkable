using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ActivityAssignedAttrToAnnouncementAssignedAttrMapper : BaseMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>
    {
        protected override void InnerMap(AnnouncementAssignedAttribute announcementAtt, ActivityAssignedAttribute activityAtt)
        {
            announcementAtt.Name = activityAtt.Name;
            announcementAtt.VisibleForStudents = activityAtt.VisibleInHomePortal;
            announcementAtt.Id = activityAtt.Id;
            announcementAtt.Text = activityAtt.Text;
            announcementAtt.AttributeId = activityAtt.AttributeId;
           
            //announcementAtt.AnnouncementRef = activityAtt.;
            
            /*
            if (activityAtt.Attachment != null)
            {
                var att = activityAtt.Attachment;
                
            }*/
        }
    }
}
