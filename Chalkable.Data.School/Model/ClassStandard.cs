using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.Data.School.Model
{
    public class ClassStandard
    {
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string STANDARD_REF_FIELD = "StandardRef";

        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StandardRef { get; set; }
    }
}
