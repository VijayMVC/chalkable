using System;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AttachmentToStiAttachmentMapper : BaseMapper<StiAttachment, Attachment>
    {
        protected override void InnerMap(StiAttachment returnObj, Attachment sourceObj)
        {
            if (sourceObj.SisAttachmentId.HasValue)
                returnObj.AttachmentId = sourceObj.SisAttachmentId.Value;
            returnObj.CrocoDocId = !string.IsNullOrWhiteSpace(sourceObj.Uuid) ? Guid.Parse(sourceObj.Uuid) : (Guid?)null;
            returnObj.MimeType = sourceObj.MimeType;
            returnObj.Name = sourceObj.Name;
        }
    }

    public class StiAttachmentToAttachmentMapper : BaseMapper<Attachment, StiAttachment>
    {
        protected override void InnerMap(Attachment returnObj, StiAttachment sourceObj)
        {
            returnObj.SisAttachmentId = sourceObj.AttachmentId;
            returnObj.Name = sourceObj.Name;
            returnObj.MimeType = sourceObj.MimeType;
        }
    }
}
