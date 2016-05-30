using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.API.Models.AcademicBenchmark
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public short Level { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsDeepest { get; set; }
        public bool IsActive { get; set; }
    }
}
