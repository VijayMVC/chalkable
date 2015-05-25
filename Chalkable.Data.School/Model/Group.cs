using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Group
    {
        public const string ID_FIELD = "Id";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StudentGroup
    {
        public int GroupRef { get; set; }
        public int StudentRef { get; set; }
    }
}
