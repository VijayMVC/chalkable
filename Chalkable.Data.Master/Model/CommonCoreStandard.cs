using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class CommonCoreStandard
    {
        public const string ID_FIELD = "Id";
        public const string PARENT_STANDARD_REF_FIELD = "ParentStandardRef";
        public const string STANDARD_CATEGORY_REF_FIELD = "StandardCategoryRef";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid? ParentStandardRef { get; set; }
        public Guid StandardCategoryRef { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
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
        [PrimaryKeyFieldAttr]
        public Guid CCStandardRef { get; set; }
        [PrimaryKeyFieldAttr]
        public Guid AcademicBenchmarkId { get; set; }
    }
}
