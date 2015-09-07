using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model.Chlk
{
    public class CommonCoreStandard
    {
        public const string ID_FIELD = "Id";
        public const string PARENT_STANDARD_REF_FIELD = "ParentStandardRef";
        public const string STANDARD_CATEGORY_REF_FIELD = "StandardCategoryRef";
        public const string ACADEMIC_BENCHMARK_ID_FIELD = "AcademicBenchmarkId";
        public const string CODE_FIELD = "Code";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid? ParentStandardRef { get; set; }
        public Guid StandardCategoryRef { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        [NotDbFieldAttr]
        public Guid? AcademicBenchmarkId { get; set; }
    }
    
    public class CommonCoreStandardCategory
    {
        public const string ID_FIELD = "Id";
        
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ABToCCMapping
    {
        public const string CC_STANADARD_REF_FIELD = "CCStandardRef";
        public const string ACADEMIC_BENCHMARK_ID_FIELD = "AcademicBenchmarkId";
        
        [PrimaryKeyFieldAttr]
        public Guid CCStandardRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid AcademicBenchmarkId { get; set; }
    }

    public class AbToCCMappingDetails : ABToCCMapping
    {
        [DataEntityAttr]
        public CommonCoreStandard Standard { get; set; }
    }
}
