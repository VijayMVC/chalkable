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
                    GradeLevels = GradeLevelViewData.Create(groupExplorer.GradeLevels).OrderBy(x=>x.Number).ToList(),
                    Schools = groupExplorer.Schools.Select(x=> new SchoolForGroupViewData
                        {
                            Id = x.Id,
                            Name = x.Name,
                            SchoolYearId = x.SchoolYearRef
                        }).ToList() ,
                    Members = groupExplorer.GroupMembers.Select(GroupMemberViewData.Create).ToList()
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
        public enum GroupMemberState
        {
            NoStudents = 0,
            HasStudents = 1,
            HasAllStudents = 2
        }

        public int GroupId { get; set; }
        public int GradeLevelId { get; set; }
        public int SchoolYearId { get; set; }
        public int SchoolId { get; set; }
        public GroupMemberState MemberState { get; set; }

        public static GroupMemberViewData Create(GroupMember groupMember)
        {
            var res = new GroupMemberViewData
                {
                    GradeLevelId = groupMember.GradeLevelRef,
                    GroupId = groupMember.GroupRef,
                    SchoolId = groupMember.SchoolRef,
                    SchoolYearId = groupMember.SchoolYearRef,
                    MemberState = groupMember.StudentsGroupInGradeLevel > 0 ? GroupMemberState.HasStudents :  GroupMemberState.NoStudents
                };
            if (groupMember.StudentsGroupInGradeLevel == groupMember.StudentsInGradeLevel)
                res.MemberState = GroupMemberState.HasAllStudents;
            return res;
        }
    }

}