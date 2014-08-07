using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class StiAttachmentToAnnouncementAttMapper : BaseMapper<AnnouncementAttachment, StiAttachment>
    {
        protected override void InnerMap(AnnouncementAttachment announcementAtt, StiAttachment activityAtt)
        {
            announcementAtt.Name = activityAtt.Name;
            announcementAtt.SisAttachmentId = activityAtt.AttachmentId;
            if (activityAtt.CrocoDocId.HasValue)
                announcementAtt.Uuid = activityAtt.CrocoDocId.Value.ToString();
            if (activityAtt is ActivityAttachment)
                announcementAtt.SisActivityId = (activityAtt as ActivityAttachment).ActivityId;
        }
    }
}
