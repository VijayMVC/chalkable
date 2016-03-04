using System;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkStandard : AcademicBenchmarkShortStandard
    {
        public AcademicBenchmarkAuthority Authority { get; set; }
        public AcademicBenchmarkDocument Document { get; set; }
    }
}
