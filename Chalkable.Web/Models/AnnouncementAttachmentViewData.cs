using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models
{
    public class AnnouncementAttachmentViewData 
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public DateTime AttachedDate { get; set; }
        public int Order { get; set; }
        public AttachmentViewData Attachment { get; set; }

        
        public static AnnouncementAttachmentViewData Create(AnnouncementAttachmentInfo announcementAttachmentInfo, bool isOwner)
        {
            return new AnnouncementAttachmentViewData
            {
                AttachedDate = announcementAttachmentInfo.AnnouncementAttachment.AttachedDate,
                Id = announcementAttachmentInfo.AnnouncementAttachment.Id,
                Order = announcementAttachmentInfo.AnnouncementAttachment.Order,
                Attachment = AttachmentViewData.Create(announcementAttachmentInfo.AttachmentInfo, isOwner),
                AnnouncementId = announcementAttachmentInfo.AnnouncementAttachment.AnnouncementRef
            };
        }
        public static IList<AnnouncementAttachmentViewData> Create(IList<AnnouncementAttachmentInfo> announcementAttachments, int personId)
        {
            return announcementAttachments.Select(x => Create(x, x.AttachmentInfo.Attachment.PersonRef == personId)).ToList();
        }
    }
}