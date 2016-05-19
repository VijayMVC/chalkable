using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class StiAttachment
    {
        /// <summary>
        /// The id of the Attachment. 
        /// </summary>
        public int AttachmentId { get; set; }

        /// <summary>
        /// The mime type of the Attachment. 
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The name of the Attachment. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Guid that identifies the attachment in CrocoDocs
        /// </summary>
        public Guid? CrocoDocId { get; set; }
    }
}
