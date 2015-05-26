using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class GroupViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<StudentViewData> Students { get; set; }
 
        public static GroupViewData Create(GroupDetails groupDetails)
        {
            return new GroupViewData
                {
                    Id = groupDetails.Id,
                    Name = groupDetails.Name,
                    Students = groupDetails.Students.Select(StudentViewData.Create).ToList()
                };
        }

        public static IList<GroupViewData> Create(IList<GroupDetails> groupDetailses)
        {
            return groupDetailses.Select(Create).ToList();
        } 
    }
}