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
        public const string STANDARD_CATEGORY_REF_FIELD = "StandardCategoryRef";

        [PrimaryKeyFieldAttr]
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid StandardCategoryRef { get; set; }
    }


    public class CC_StandardCategory
    {
        public const string ID_FIELD = "Id";
        public const string PARENT_CATEGORY_REF_FIELD = "ParentCategoryRef";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid? ParentCategoryRef { get; set; }
        public string Name { get; set; }
    }
}
