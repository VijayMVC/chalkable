using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class GroupExplorerViewData
    {
        public GroupViewData Group { get; set; }
        public IList<GradeLevelViewData> GradeLevels { get; set; }
        public IList<SchoolForGroupViewData> Schools { get; set; }
 
        public IList<GroupMemberViewData> Members { get; set; }
 
        public static GroupExplorerViewData Create(GroupExplorer groupExplorer)
        {
            return new GroupExplorerViewData
                {
                    Group = GroupViewData.Create(groupExplorer.Group),
                    GradeLevels = GradeLevelViewData.Create(groupExplorer.GradeLevels),
                    Schools = groupExplorer.Schools.Select(x=> new SchoolForGroupViewData
                        {
                            Id = x.Id,
                            Name = x.Name,
                            SchoolYearId = x.SchoolYearRef
                        }).ToList() ,
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


    public class SchoolForGroupViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SchoolYearId { get; set; }
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