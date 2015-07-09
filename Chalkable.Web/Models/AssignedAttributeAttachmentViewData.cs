using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;

namespace Chalkable.Web.Models
{
    public class AssignedAttributeAttachmentViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Type { get; set; }
        public bool StiAttachment { get; set; }
        public string Uuid { get; set; }
        public string MimeType { get; set; }

        public static AssignedAttributeAttachmentViewData Create(AnnouncementAssignedAttributeAttachment attachment, IList<AssignedAttributeAttachmentInfo> attrAttachmentInfos)
        {
            AssignedAttributeAttachmentViewData attachmentViewData = null;

            if (attachment != null)
            {
                var attachmentInfo = attrAttachmentInfos.FirstOrDefault(x => attachment.StiAttachment
                                   && x.AttributeAttachment != null &&
                                   x.AttributeAttachment.AttachmentId == attachment.AttachmentId);

                attachmentViewData = new AssignedAttributeAttachmentViewData
                {
                    Id = attachment.AttachmentId,
                    Name = attachment.AttachmentName,
                    Type = (int)MimeHelper.GetTypeByName(attachment.AttachmentName),
                    StiAttachment = attachment.StiAttachment,
                    Uuid = attachment.Uuid,
                    MimeType = attachment.MimeType

                };

                if (attachmentInfo != null)
                {
                    attachmentViewData.ThumbnailUrl = attachmentInfo.DownloadThumbnailUrl;
                    attachmentViewData.Url = attachmentInfo.DownloadDocumentUrl;
                }
                
            }

            
            return attachmentViewData;
        }

       
    }
}