using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class UserSchool
    {
        public const string USER_REF_FIELD = "UserRef";
        public const string SCHOOL_REF_FIELD = "SchoolRef";

        [PrimaryKeyFieldAttr]
        public int UserRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int SchoolRef { get; set; }
    }
}