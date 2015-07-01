using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class StiAttachmentToAnnouncementAttMapper : BaseMapper<AnnouncementAttachment, ActivityAttribute>
    {
        protected override void InnerMap(AnnouncementAttachment announcementAtt, ActivityAttribute activityAtt)
        {
            if (activityAtt.Attachment != null)
            {
                var att = activityAtt.Attachment;
                announcementAtt.Name = att.Name;
                announcementAtt.SisAttachmentId = att.AttachmentId;
                if (att.CrocoDocId.HasValue)
                    announcementAtt.Uuid = att.CrocoDocId.Value.ToString();
                announcementAtt.SisActivityId = activityAtt.ActivityId;
            }
        }
    }
}
