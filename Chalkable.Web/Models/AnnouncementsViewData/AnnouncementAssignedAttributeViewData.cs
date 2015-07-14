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
        public int AnnouncementRef { get; set; }
        public string Uuid { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }
        public AssignedAttributeAttachmentViewData AttributeAttachment { get; set; }



        public static AnnouncementAssignedAttributeViewData Create(AnnouncementAssignedAttribute attr, IList<AssignedAttributeAttachmentInfo> attrAttachmentInfos)
        {
            var result = new AnnouncementAssignedAttributeViewData
            {
                Id = attr.Id,
                Name = attr.Name,
                Text = attr.Text,
                AttributeTypeId = attr.AttributeTypeId,
                Uuid = attr.Uuid,
                VisibleForStudents = attr.VisibleForStudents,
                AnnouncementRef = attr.AnnouncementRef,
                AttributeAttachment = AssignedAttributeAttachmentViewData.Create(attr.Attachment, attrAttachmentInfos)
            };


            return result;
        }

        public static IList<AnnouncementAssignedAttributeViewData> Create(IList<AnnouncementAssignedAttribute> announcementAttributes, IList<AssignedAttributeAttachmentInfo> attrAttachmentInfos)
        {
            return announcementAttributes.Select(annAtrr => Create(annAtrr, attrAttachmentInfos)).ToList();
        }
    }
}