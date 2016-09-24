using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class Document
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
    }
}
