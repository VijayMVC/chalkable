using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Configuration;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementAssignedAttributeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AttributeTypeId { get; set; }
        public string Uuid { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }
        public AssignedAttributeAttachmentViewData AttributeAttachment { get; set; }

        public static IList<AnnouncementAssignedAttributeViewData> Create(IList<AnnouncementAssignedAttribute> announcementAttributes, IList<AssignedAttributeAttachmentInfo> attrAttachmentInfos)
        {


            var attrs = new List<AnnouncementAssignedAttributeViewData>();


            foreach (var annAtrr in announcementAttributes)
            {
                var wd = new AnnouncementAssignedAttributeViewData
                {
                    Id = annAtrr.Id,
                    Name = annAtrr.Name,
                    Text = annAtrr.Text,
                    AttributeTypeId = annAtrr.AttributeTypeId,
                    Uuid = annAtrr.Uuid,
                    VisibleForStudents = annAtrr.VisibleForStudents

                };


                if (annAtrr.SisAttributeAttachmentId.HasValue)
                {

                    var attachmentInfo = attrAttachmentInfos.FirstOrDefault(
                            x =>
                                x.AttributeAttachment != null &&
                                x.AttributeAttachment.AttachmentId == annAtrr.SisAttributeAttachmentId.Value);

                    if (attachmentInfo != null)
                    {
                        wd.AttributeAttachment = new AssignedAttributeAttachmentViewData()
                        {
                            Id = annAtrr.SisAttributeAttachmentId.Value,
                            Name = annAtrr.SisAttachmentName,
                            ThumbnailUrl = attachmentInfo.DownloadThumbnailUrl,
                            Url = attachmentInfo.DownloadDocumentUrl
                            //Type = annAtrr.SisAttachmentMimeType
                        };
                    }
                    
                        
                }

                attrs.Add(wd);
            }

            return attrs;
        } 
    }
}