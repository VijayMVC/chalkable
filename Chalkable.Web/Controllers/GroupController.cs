using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class GroupController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult GroupsList()
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var groups = SchoolLocator.GroupService.GetGroupsDetails(Context.PersonId.Value);
            return Json(GroupViewData.Create(groups));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult CreateGroup(string name)
        {
            SchoolLocator.GroupService.AddGroup(name);
            return GroupsList();
        }
        
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DeleteGroup(int groupId)
        {
            SchoolLocator.GroupService.DeleteGroup(groupId);
            return GroupsList();
        }
    }
}