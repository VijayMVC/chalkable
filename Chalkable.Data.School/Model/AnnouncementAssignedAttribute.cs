using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAssignedAttribute
    {
        /*
        public const string ID_FIELD = "Id";
        public const string ANNOUNCEMENT_REF_FIELD = "AnnouncementRef";
        public const string PERSON_REF_FIELD = "PersonRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int PersonRef { get; set; }
        public int AnnouncementRef { get; set; }
        public DateTime AttachedDate { get; set; }
        public string Uuid { get; set; }
        public int Order { get; set; }

        [NotDbFieldAttr]
        public int? SisActivityId { get; set; }

        public int? SisAttachmentId { get; set; }*/


        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public short AttributeId { get; set; }
        
        //public StiAttachment Attachment { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }

    }
}
