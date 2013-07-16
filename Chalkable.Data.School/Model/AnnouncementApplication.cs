using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementApplication
    {
        public Guid Id { get; set; }
        public Guid AnnouncementRef { get; set; }
        public Guid ApplicationRef { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }
    }
}
