using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Tools;

namespace Chalkable.Web.Models
{
    public class AnnouncementAttachmentViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AttachedDate { get; set; }
        public bool IsOwner { get; set; }
        public int Type { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Order { get; set; }

        private const string PDF_EXT = ".pdf";
        private const string CROCODOC_API_URL_FORMAT = "download/document?uuid={0}&pdf={1}&annotated={2}&token={3}";
        private const string CROCODOC_THUMBNAIL_URL_FORMAT = "download/thumbnail?token={0}&uuid={1}&size={2}x{3}";
        private const string TRUE = "true";

        public static AnnouncementAttachmentViewData Create(AnnouncementAttachment attachment, bool isOwner)
        {
            return new AnnouncementAttachmentViewData
            {
                AttachedDate = attachment.AttachedDate,
                Id = attachment.Id,
                Name = attachment.Name,
                IsOwner = isOwner,
                Type = (int)MimeHelper.GetTypeByName(attachment.Name),
                Order = attachment.Order
            };
        }

        public static AnnouncementAttachmentViewData Create(AnnouncementAttachmentInfo announcementAttachment, bool isOwner)
        {
            var res = Create(announcementAttachment.Attachment, isOwner);

            if (!(string.IsNullOrEmpty(announcementAttachment.Token) || string.IsNullOrEmpty(announcementAttachment.StorageUrl)))
            {
                var ispdf = IsPdf(announcementAttachment.Attachment);
                res.Url = string.Format(announcementAttachment.CrocodocApiUrl + CROCODOC_API_URL_FORMAT,
                                        announcementAttachment.Attachment.Uuid, ispdf, TRUE, announcementAttachment.Token);

                res.ThumbnailUrl = string.Format(announcementAttachment.CrocodocApiUrl + CROCODOC_THUMBNAIL_URL_FORMAT, announcementAttachment.Token,
                                                 announcementAttachment.Attachment.Uuid, announcementAttachment.DocWidth, announcementAttachment.DocHeigth);
            }
            return res;
        }

        public static IList<AnnouncementAttachmentViewData> Create(IList<AnnouncementAttachmentInfo> announcementAttachments, int personId)
        {
            var res = announcementAttachments.Select(x => Create(x, x.Attachment.PersonRef == personId));
            return res.ToList();
        }


        private static bool IsPdf(AnnouncementAttachment announcementAttachment)
        {
            return PDF_EXT == (Path.GetExtension(announcementAttachment.Name) ?? string.Empty).ToLower();
        }
    }
}