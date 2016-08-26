using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.Model
{
    public class Standard
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string Label { get; set; }
        public bool IsDeepest { get; set; }
        public int Level { get; set; }
        public string Seq { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }
        public DateTime DateModified { get; set; }
        public string ExtDescription { get; set; }
        public int AdoptYear { get; set; }
        public Guid AuthorityRef { get; set; }
        public Guid DocumentRef { get; set; }
        public string SubjectRef { get; set; }
        public Guid SubjectDocRef { get; set; }
        public Guid CourseRef { get; set; }
        public Guid ParentRef { get; set; }
        public string GradeLevelLoRef { get; set; }
        public string GradeLevelHiRef { get; set; }
    }
}
