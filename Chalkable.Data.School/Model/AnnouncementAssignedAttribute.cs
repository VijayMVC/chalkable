using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAssignedAttribute
    {
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";

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

        [NotDbFieldAttr]
        public int? SisActivityId { get; set; }
        [NotDbFieldAttr]
        public int? SisAttributeId { get; set; }

    }
}
