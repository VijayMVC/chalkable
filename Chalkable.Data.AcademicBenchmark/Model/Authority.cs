using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class Authority
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
