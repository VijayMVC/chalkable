using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class GroupViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasStudents { get; set; }

        protected GroupViewData(){}
        protected GroupViewData(Group gGroup)
        {
            Id = gGroup.Id;
            Name = gGroup.Name;
            HasStudents = gGroup.HasStudents;
        }
        public static GroupViewData Create(Group gGroup)
        {
            return new GroupViewData(gGroup);
        }
    }
}