using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    class Subject
    {
        [PrimaryKeyFieldAttr]
        public string Code { get; set; }
        public string Description { get; set; }
        public string Broad { get; set; }
    }
}
