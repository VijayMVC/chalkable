using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementRecipientData
    {
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int AnnouncementRef { get; set; }
        public int PersonRef { get; set; }
        public bool Starred { get; set; }
        public bool StarredAutomatically { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
