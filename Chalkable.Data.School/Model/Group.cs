using System.Collections.Generic;
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
        public int OwnerRef { get; set; }
    }

    public class GroupDetails : Group
    {
        public IList<Student> Students { get; set; }
    }

    public class StudentGroup
    {
        [PrimaryKeyFieldAttr]
        public int GroupRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
    }
}
