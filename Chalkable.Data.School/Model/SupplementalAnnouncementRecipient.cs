using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SupplementalAnnouncementRecipient
    {
        public const string SUPPLEMENTAL_ANNONCEMENT_REF_FIELD = nameof(SupplementalAnnouncementRef);
        public const string STUDENT_REF_FIELD = nameof(StudentRef);

        [PrimaryKeyFieldAttr]
        public int SupplementalAnnouncementRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }

        [NotDbFieldAttr]
        public Person Recipient { get; set; }
    }
}
