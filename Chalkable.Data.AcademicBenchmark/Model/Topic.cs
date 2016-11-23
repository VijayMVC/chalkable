using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class Topic
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsDeepest { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }
        public int AdoptYear { get; set; }
        public string SubjectRef { get; set; }
        public Guid SubjectDocRef { get; set; }
        public Guid CourseRef { get; set; }
        public Guid? ParentRef { get; set; }
        public string GradeLevelLoRef { get; set; }
        public string GradeLevelHiRef { get; set; }
    }
}
