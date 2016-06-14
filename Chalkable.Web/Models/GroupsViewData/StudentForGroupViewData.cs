using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class StudentForGroupViewData : StudentViewData
    {
        public bool AssignedToGroup { get; set; }
        protected StudentForGroupViewData(StudentForGroup student) : base(student)
        {
            AssignedToGroup = student.AssignedToGroup;
        }

        public static IList<StudentForGroupViewData> Create(IList<StudentForGroup> studentForGroups)
        {
            return studentForGroups.Select(x => new StudentForGroupViewData(x)).ToList();
        }
    }
}