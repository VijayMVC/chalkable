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
        public Guid Id { get; set; }
        public Guid AnnouncementRef { get; set; }
        public Guid PersonRef { get; set; }
        public bool Starred { get; set; }
        public bool StarredAutomatically { get; set; }

        [DataEntityAttr]
        public Announcement Announcement { get; set; }
        [DataEntityAttr]
        public Person Person { get; set; }

    }
}
