using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkShortStandard
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public Guid ParentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
    }
}
