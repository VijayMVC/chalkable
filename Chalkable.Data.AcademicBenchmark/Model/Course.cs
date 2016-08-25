using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class Course
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
