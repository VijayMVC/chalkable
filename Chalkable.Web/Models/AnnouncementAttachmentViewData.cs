using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;

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
        public bool IsTeacherAttachment { get; set; }

        public static AnnouncementAttachmentViewData Create(AnnouncementAttachment attachment, bool isOwner, bool isTeacherAttachment)
        {
            return new AnnouncementAttachmentViewData
            {
                AttachedDate = attachment.AttachedDate,
                Id = attachment.Id,
                Name = attachment.Name,
                IsOwner = isOwner,
                Type = (int)MimeHelper.GetTypeByName(attachment.Name),
                Order = attachment.Order,
                IsTeacherAttachment = isTeacherAttachment

            };
        }

        public static AnnouncementAttachmentViewData Create(AnnouncementAttachmentInfo announcementAttachment, bool isOwner)
        {
            var res = Create(announcementAttachment.Attachment, isOwner, announcementAttachment.IsTeacherAttachment);
            res.Url = announcementAttachment.DownloadDocumentUrl;
            res.ThumbnailUrl = announcementAttachment.DownloadThumbnailUrl;
            return res;
        }
        public static IList<AnnouncementAttachmentViewData> Create(IList<AnnouncementAttachmentInfo> announcementAttachments, int personId)
        {
            var res = announcementAttachments.Select(x => Create(x, x.Attachment.PersonRef == personId));
            return res.ToList();
        }
    }
}