using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class AnnouncementCommentViewData
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public AttachmentViewData Attachment { get; set; }
        public ShortPersonViewData Owner { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime TimePosted { get; set; }
        public string Text { get; set; }
        public bool Hidden { get; set; }
        public bool Deleted { get; set; }
        public IList<AnnouncementCommentViewData> SubComments { get; set; } 

        public static AnnouncementCommentViewData Create(AnnouncementComment announcementComment, AttachmentInfo attachmentInfo, int currentPersonId)
        {
            return new AnnouncementCommentViewData
            {
                Id = announcementComment.Id,
                AnnouncementId = announcementComment.AnnouncementRef,
                Attachment = attachmentInfo != null ? AttachmentViewData.Create(attachmentInfo, currentPersonId) : null,
                ParentCommentId = announcementComment.ParentCommentRef,
                Owner = ShortPersonViewData.Create(announcementComment.Person),
                TimePosted = announcementComment.PostedDate,
                Text = announcementComment.Text,
                Hidden = announcementComment.Hidden,
                Deleted = announcementComment.Deleted
            };
        }
    }
}