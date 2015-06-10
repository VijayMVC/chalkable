using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AnnouncementAttToActivityAttMapper : BaseMapper<ActivityAttachment, AnnouncementAttachment>
    {
        protected override void InnerMap(ActivityAttachment activityAtt, AnnouncementAttachment announcementAtt)
        {
            if (announcementAtt.SisAttachmentId.HasValue)
                activityAtt.AttachmentId = announcementAtt.SisAttachmentId.Value;
            if (!string.IsNullOrEmpty(announcementAtt.Uuid))
                activityAtt.CrocoDocId = new Guid(announcementAtt.Uuid);
            activityAtt.Name = announcementAtt.Name;
        }
    }
}
