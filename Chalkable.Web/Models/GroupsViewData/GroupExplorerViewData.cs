using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.SchoolsViewData;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class GroupExplorerViewData
    {
        public GroupViewData Group { get; set; }
        public IList<GradeLevelViewData> GradeLevels { get; set; }
        public IList<SchoolViewData> Schools { get; set; }
 
        public IList<GroupMemberViewData> Members { get; set; }
 
        public static GroupExplorerViewData Create(GroupExplorer groupExplorer)
        {
            return new GroupExplorerViewData
                {
                    Group = GroupViewData.Create(groupExplorer.Group),
                    GradeLevels = GradeLevelViewData.Create(groupExplorer.GradeLevels),
                    Members = groupExplorer.GroupMembers.Select(x => new GroupMemberViewData
                        {
                            GradeLevelId = x.GradeLevelRef,
                            GroupId = x.GroupRef,
                            SchoolId = x.SchoolRef,
                            SchoolYearId = x.SchoolYearRef,
                            IncludeAllStudentsInGradeLevel = x.StudentsGroupInGradeLevel == x.StudentsInGradeLevel
                        }).ToList()
                };
        }
    }


    public class GroupMemberViewData
    {
        public int GroupId { get; set; }
        public int GradeLevelId { get; set; }
        public int SchoolYearId { get; set; }
        public int SchoolId { get; set; }
        public bool IncludeAllStudentsInGradeLevel { get; set; }
    }

}