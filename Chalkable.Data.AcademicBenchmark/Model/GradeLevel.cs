using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class GradeLevel
    {
        [PrimaryKeyFieldAttr]
        public string Code { get; set; }
        public string Description { get; set; }
        public string Low { get; set; }
        public string High { get; set; }
    }
}
