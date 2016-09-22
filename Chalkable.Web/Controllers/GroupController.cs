using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.GroupsViewData;

namespace Chalkable.Web.Controllers
{
    public class GroupController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult GroupsList(string filter = null)
        {
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var groups = SchoolLocator.GroupService.GetGroups(Context.PersonId.Value, filter);
            return Json(groups.Select(GroupViewData.Create).ToList());
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult Info(int groupId)
        {
            var students = SchoolLocator.GroupService.Info(groupId);
            return Json(GroupInfoViewData.Create(students));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult CreateGroup(string name, IntList studentsIds)
        {
            return Json(GroupViewData.Create(SchoolLocator.GroupService.AddGroup(name, studentsIds)));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult EditGroup(int groupId, string name, IntList studentsIds)
        {
            return Json(GroupViewData.Create(SchoolLocator.GroupService.EditGroup(groupId, name, studentsIds)));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DeleteGroup(int groupId)
        {
            SchoolLocator.GroupService.DeleteGroup(groupId);
            return GroupsList();
        }
    }
}