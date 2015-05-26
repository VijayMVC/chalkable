using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Group
    {
        public const string ID_FIELD = "Id";
        public const string OWNER_REF_FIELD = "OwnerRef";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerRef { get; set; }
    }

    public class GroupDetails : Group
    {
        public IList<StudentDetails> Students { get; set; }
    }

    public class StudentGroup
    {
        public const string GROUP_REF_FIELD = "GroupRef";
        public const string STUDENT_REF_FIELD = "StudentRef";
        
        [PrimaryKeyFieldAttr]
        public int GroupRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int StudentRef { get; set; }
    }

    public class StudentGroupComplex
    {
        [DataEntityAttr]
        public Group Group { get; set; }
        [DataEntityAttr]
        public StudentDetails Student { get; set; }
    }
}
