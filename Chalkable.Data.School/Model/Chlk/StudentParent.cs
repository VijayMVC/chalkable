using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.Model.Chlk
{
    public class StudentParent
    {
        public int Id { get; set; }
        public int ParentRef { get; set; }
        public int StudentRef { get; set; }
    }

    public class StudentParentDetails : StudentParent
    {
        public PersonDetails Parent { get; set; }
        public PersonDetails Student { get; set; }
    }
}
