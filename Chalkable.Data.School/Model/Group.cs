using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Group
    {
        public const string ID_FIELD = "Id";
        public const string OWNER_REF_FIELD = "OwnerRef";
        public const string STUDENT_COUNT_FIELD = "StudentCount";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerRef { get; set; }

        [NotDbFieldAttr]
        public int StudentCount { get; set; }
        [NotDbFieldAttr]
        public bool HasStudents { get { return StudentCount > 0; } }
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
        public Student Student { get; set; }
    }


    public class GroupExplorer
    {
        public Group Group { get; set; }
        public IList<SchoolForGroup> Schools { get; set; }
        public IList<GradeLevel> GradeLevels { get; set; }
        public IList<GroupMember> GroupMembers { get; set; }
    }

    public class SchoolForGroup : School
    {
        public int SchoolYearRef { get; set; }
    }

    public class GroupMember
    {
        public int GroupRef { get; set; }
        public int SchoolRef { get; set; }
        public int GradeLevelRef { get; set; }
        public int SchoolYearRef { get; set; }

        public int StudentsInSchool { get; set; }
        public int StudentsGroupInSchool { get; set; }
        public int StudentsInGradeLevel { get; set; }
        public int StudentsGroupInGradeLevel { get; set; }
    }

    public class StudentForGroup : Student
    {
        public int? GroupRef { get; set; }

        [NotDbFieldAttr]
        public bool AssignedToGroup
        {
            get { return GroupRef != null; }
        }
    }
}
