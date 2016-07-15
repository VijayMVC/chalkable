using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityAttachment : StiAttachment
    {
        /// <summary>
        /// The Id of the activity
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// The id of the ActivityAssignedAttribute. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The text of the attribute
        /// </summary>
        public string Text { get; set; }

        public static ActivityAttachment Create(int activityId, StiAttachment attachment, string text)
        {
            return new ActivityAttachment
                {
                    ActivityId = activityId,
                    AttachmentId = attachment.AttachmentId,
                    CrocoDocId = attachment.CrocoDocId,
                    MimeType = attachment.MimeType,
                    Name = attachment.Name,
                    Text = text
                };
        }
    }
}
