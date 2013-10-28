namespace Chalkable.Data.School.Model
{
    public class SchoolPerson
    {
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string ROLE_REF = "RoleRef";

        public int SchoolRef { get; set; }
        public int PersonRef { get; set; }
        public int RoleRef { get; set; }
    }
}