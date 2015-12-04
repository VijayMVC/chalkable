using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAssignedAttribute
    {
        public const string VW_ANNOUNCEMENT_ASSIGNED_ATTRIBUTE = "vwAnnouncementAssignedAttribute";
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string SIS_ATTRIBUTE_ATTACHMENT_ID = "SisAttributeAttachmentId";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int AttributeTypeId { get; set; }
        public int? AttachmentRef { get; set; }
 
        public string Name { get; set; }
        private string _text;

        public string Text
        {
            get { return _text ?? ""; }
            set { _text = value; }
        }
        public bool VisibleForStudents { get; set; }
       
        public int? SisActivityAssignedAttributeId { get; set; }
        public int? SisActivityId { get; set; }

        [NotDbFieldAttr]
        public bool IsStiAttribute => SisActivityId.HasValue && SisActivityAssignedAttributeId.HasValue;

        [NotDbFieldAttr]
        public Attachment Attachment { get; set; }
    }

}
