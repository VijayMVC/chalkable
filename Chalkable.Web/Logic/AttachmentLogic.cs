using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Logic
{
    public static class AttachmentLogic
    {
        public static IList<AnnouncementAttachmentInfo> PrepareAttachmentsInfo(IList<AnnouncementAttachment> announcementAttachments, IList<int> teachersIds = null)
        {
            var storageUrl = PreferenceService.Get(Preference.CROCODOC_URL).Value;
            var token = PreferenceService.Get(Preference.CROCODOC_TOKEN).Value;
            var apiUrl = PreferenceService.Get(Preference.CROCODOC_API_URL).Value;
            return announcementAttachments.Select(x => new AnnouncementAttachmentInfo
            {
                Attachment = x,
                StorageUrl = storageUrl,
                CrocodocApiUrl = apiUrl,
                Token = token,
                DocWidth = AnnouncementAttachment.DOCUMENT_DEFAULT_WIDTH,
                DocHeigth = AnnouncementAttachment.DOCUMENT_DEFAULT_HEIGHT,
                IsTeacherAttachment = teachersIds != null && teachersIds.Contains(x.PersonRef)
            }).ToList();
        }

        public static AnnouncementAttachmentInfo PrepareAttachmentInfo(AnnouncementAttachment announcementAttachment)
        {
            var res = PrepareAttachmentsInfo(new List<AnnouncementAttachment> {announcementAttachment}, new List<int>());
            return res.First();
        }
    }


    public class AnnouncementAttachmentInfo
    {
        public AnnouncementAttachment Attachment { get; set; }
        public string StorageUrl { get; set; }
        public string CrocodocApiUrl { get; set; }
        public string Token { get; set; }
        public int DocWidth { get; set; }
        public int DocHeigth { get; set; }
        public bool IsTeacherAttachment { get; set; }
    }
}