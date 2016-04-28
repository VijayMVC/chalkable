using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SchoolPerson
    {
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string ROLE_REF = "RoleRef";

        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        public int RoleRef { get; set; }
    }
}