using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class GroupViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        protected GroupViewData(){}
        protected GroupViewData(Group gGroup)
        {
            Id = gGroup.Id;
            Name = gGroup.Name;
        }
        public static GroupViewData Create(Group gGroup)
        {
            return new GroupViewData(gGroup);
        }
    }
}