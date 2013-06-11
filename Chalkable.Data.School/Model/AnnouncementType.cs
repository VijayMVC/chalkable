using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementType
    {
        public Guid Id { get; set; }
        public bool IsSystem { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Gradable { get; set; }
    }
}
