using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
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
