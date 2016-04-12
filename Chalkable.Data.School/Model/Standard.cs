using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Standard
    {       
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int? ParentStandardRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectRef { get; set; }
        public int? LowerGradeLevelRef { get; set; }
        public int? UpperGradeLevelRef { get; set; }
        public bool IsActive { get; set; }
        public Guid? AcademicBenchmarkId { get; set; }
        [NotDbFieldAttr]
        public bool IsDeepest { get; set; }
    }
    
    public class StandardTreePath
    {
        public IList<Standard> AllStandards { get; set; }
        public IList<Standard> Path { get; set; } 
    }

    public class StandardTreeItem
    {
        [DataEntityAttr]
        public Standard Standard { get; set; }
        public bool IsSelected { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
    }
}
