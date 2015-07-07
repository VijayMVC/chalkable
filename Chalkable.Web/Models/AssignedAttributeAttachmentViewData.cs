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

        public static AssignedAttributeAttachmentViewData Create(AssignedAttributeAttachment attachment)
        {
            return new AssignedAttributeAttachmentViewData
            {
                Id = attachment.AttachmentId,
                Name = attachment.Name,
                Type = (int)MimeHelper.GetTypeByName(attachment.Name)
            };
        }

        public static AssignedAttributeAttachmentViewData Create(AssignedAttributeAttachmentInfo attributeAttachment)
        {
            var res = Create(attributeAttachment.AttributeAttachment);
            res.Url = attributeAttachment.DownloadDocumentUrl;
            res.ThumbnailUrl = attributeAttachment.DownloadThumbnailUrl;
            return res;
        }
        public static IList<AssignedAttributeAttachmentViewData> Create(IList<AssignedAttributeAttachmentInfo> attributeAttachments)
        {
            var res = attributeAttachments.Select(Create);
            return res.ToList();
        }
    }
}