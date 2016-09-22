using System.Collections.Generic;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GroupsViewData
{
    public class GroupInfoViewData : GroupViewData
    {
        private IList<ShortPersonViewData> Students { get; set; }

        protected GroupInfoViewData(GroupInfo groupInfo) : base(groupInfo)
        {
            Students = ShortPersonViewData.Create(groupInfo.Students);
        }
        public static GroupInfoViewData Create(GroupInfo gGroup)
        {
            return new GroupInfoViewData(gGroup);
        }
    }
}