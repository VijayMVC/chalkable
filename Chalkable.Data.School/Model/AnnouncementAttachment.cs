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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PersonRef { get; set; }
        public Guid AnnouncementRef { get; set; }
        public DateTime AttachedDate { get; set; }
        public string Uuid { get; set; }
        public int Order { get; set; }

        [DataEntityAttr]
        public Person Person { get; set; }
        [DataEntityAttr]
        public Announcement Announcement { get; set; }

    }
}
