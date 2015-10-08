using System;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Web;

namespace Chalkable.Web.Models
{
    public class AttachmentViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Type { get; set; }
        public bool StiAttachment { get; set; }
        public string Uuid { get; set; }
        public string MimeType { get; set; }
        public DateTime Uploaded { get; set; }
        public DateTime LastAttached { get; set; }
        public bool IsOwner { get; set; }
        public bool IsTeacherAttachment { get; set; }

        public static AttachmentViewData Create(AttachmentInfo attachmentInfo, bool isOwner)
        {
            return new AttachmentViewData
            {
                Id = attachmentInfo.Attachment.Id,
                Name = attachmentInfo.Attachment.Name,
                MimeType = attachmentInfo.Attachment.MimeType,
                Type = (int)MimeHelper.GetTypeByName(attachmentInfo.Attachment.Name),
                Uuid = attachmentInfo.Attachment.Uuid,
                Uploaded = attachmentInfo.Attachment.UploadedDate,
                LastAttached = attachmentInfo.Attachment.LastAttachedDate,
                StiAttachment = attachmentInfo.Attachment.IsStiAttachment,
                ThumbnailUrl = attachmentInfo.DownloadThumbnailUrl,
                Url = attachmentInfo.DownloadDocumentUrl,
                IsTeacherAttachment = attachmentInfo.IsTeacherAttachment,
                IsOwner = isOwner
            };
        }

       
        public static AttachmentViewData Create(AttachmentInfo attachmentInfo, int? currentPersonId = null)
        {
            return Create(attachmentInfo, currentPersonId == attachmentInfo.Attachment.PersonRef);
        }
    }
}