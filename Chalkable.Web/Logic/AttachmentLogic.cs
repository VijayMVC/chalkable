using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Logic
{
    public static class AttachmentLogic
    {
        public static IList<AnnouncementAttachmentInfo> PrepareAttachmentsInfo(IList<AnnouncementAttachment> announcementAttachments, ICrocodocService crocodocService, IList<int> itemOwnerIds = null)
        {
            int docWidth = AnnouncementAttachment.DOCUMENT_DEFAULT_WIDTH,
                docHeight = AnnouncementAttachment.DOCUMENT_DEFAULT_HEIGHT;
            return announcementAttachments.Select(annAtt => new AnnouncementAttachmentInfo()
                {
                    Attachment = annAtt, 
                    DocWidth = docWidth, 
                    DocHeigth = docHeight, 
                    IsTeacherAttachment = itemOwnerIds != null && itemOwnerIds.Contains(annAtt.PersonRef), 
                    DownloadDocumentUrl = crocodocService.BuildDownloadDocumentUrl(annAtt.Uuid, annAtt.Name), 
                    DownloadThumbnailUrl = crocodocService.BuildDownloadhumbnailUrl(annAtt.Uuid, docWidth, docHeight)
                }).ToList();
        }

        public static AnnouncementAttachmentInfo PrepareAttachmentInfo(AnnouncementAttachment announcementAttachment, ICrocodocService crocodocService)
        {
            var res = PrepareAttachmentsInfo(new List<AnnouncementAttachment> { announcementAttachment }, crocodocService, new List<int>());
            return res.First();
        }
    }


    public class AnnouncementAttachmentInfo
    {
        public AnnouncementAttachment Attachment { get; set; }
        public int DocWidth { get; set; }
        public int DocHeigth { get; set; }
        public bool IsTeacherAttachment { get; set; }
        public string DownloadDocumentUrl { get; set; }
        public string DownloadThumbnailUrl { get; set; }
    }
}