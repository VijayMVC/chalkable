using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    class SubjectDoc
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
