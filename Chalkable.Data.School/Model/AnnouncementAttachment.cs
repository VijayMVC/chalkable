using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementAttachment
    {
        public const int DOCUMENT_DEFAULT_WIDTH = 110;
        public const int DOCUMENT_DEFAULT_HEIGHT = 170;

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
        public int SisActivityId { get; set; }

    }
}
