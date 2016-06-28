using System;
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
        public AnnouncementCommentViewData ParentComment { get; set; }
        public DateTime PostedDate { get; set; }
        public string Text { get; set; }
        public bool Hidden { get; set; }
        public bool Deleted { get; set; }

        public static AnnouncementCommentViewData Create(AnnouncementComment announcementComment)
        {
            return new AnnouncementCommentViewData
            {
                Id = announcementComment.Id,
                AnnouncementId = announcementComment.AnnouncementRef,
                //Attachment = AttachmentViewData.Create(announcementComment.Attachment, true),
                ParentComment =
                    announcementComment.ParentComment != null ? Create(announcementComment.ParentComment) : null,
                Owner = ShortPersonViewData.Create(announcementComment.Person),
                PostedDate = announcementComment.PostedDate,
                Text = announcementComment.Text,
                Hidden = announcementComment.Hiddent,
                Deleted = announcementComment.Deleted
            };
        }
    }
}