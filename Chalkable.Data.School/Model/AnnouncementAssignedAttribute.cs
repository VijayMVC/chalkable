using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAssignedAttribute
    {
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string SIS_ATTRIBUTE_ATTACHMENT_ID = "SisAttributeAttachmentId";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int AttributeTypeId { get; set; }
        
        public string Name { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }

        public string Uuid { get; set; }
        public int? SisAttributeAttachmentId { get; set; }
        public string SisAttachmentName { get; set; }
        public string SisAttachmentMimeType { get; set; }
        public int? SisActivityAssignedAttributeId { get; set; }

        [NotDbFieldAttr]
        public AnnouncementAssignedAttributeAttachment Attachment
        {
            get
            {
                AnnouncementAssignedAttributeAttachment attachment = null;
                if (SisAttributeAttachmentId.HasValue)
                {
                    attachment = new AnnouncementAssignedAttributeAttachment
                    {
                        AttachmentId = SisAttributeAttachmentId.Value,
                        AttachmentName = SisAttachmentName,
                        Uuid = Uuid,
                        StiAttachment = true
                    };
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(SisAttachmentName))
                    {
                        attachment = new AnnouncementAssignedAttributeAttachment()
                        {
                            AttachmentId = Id, // local attachments have same id as attribute
                            Uuid = Uuid,
                            StiAttachment = false,
                            AttachmentName = SisAttachmentName
                        };
                    }
                }

                return attachment;
            }
        }

        


        public static AnnouncementAssignedAttribute Create(AssignedAttributeInputModel attributeInputModel)
        {
            var attribute = new AnnouncementAssignedAttribute
            {
                AnnouncementRef = attributeInputModel.AnnouncementRef,
                AttributeTypeId = attributeInputModel.AttributeTypeId,
                Id = attributeInputModel.Id,
                Name = attributeInputModel.Name,
                Text = attributeInputModel.Text,
                VisibleForStudents = attributeInputModel.VisibleForStudents
            };

            if (attributeInputModel.AttributeAttachment != null)
            {
                attribute.SisAttributeAttachmentId = attributeInputModel.AttributeAttachment.AttachmentId;
                attribute.SisAttachmentName = attributeInputModel.AttributeAttachment.AttachmentName;
                attribute.SisAttachmentMimeType = attributeInputModel.AttributeAttachment.MimeType;
            }

            return attribute;
        }


        public static IList<AnnouncementAssignedAttribute> Create(IList<AssignedAttributeInputModel> attributes)
        {
            return attributes != null ? attributes.Select(Create).ToList() : new List<AnnouncementAssignedAttribute>();
        } 

    }

    public class AnnouncementAssignedAttributeAttachment
    {
        public bool StiAttachment { get; set; }
        public int AttachmentId { get; set; }
        public string Uuid { get; set; }
        public string AttachmentName { get; set; }
        public string MimeType { get; set; }
    }

    public class AssignedAttributeInputModel
    {
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int AttributeTypeId { get; set; }
        
        public string Name { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }

        public AnnouncementAssignedAttributeAttachment AttributeAttachment { get; set; }


    }
}
