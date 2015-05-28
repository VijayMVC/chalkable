using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
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
            var groups = SchoolLocator.GroupService.GetGroups(Context.PersonId.Value);
            return Json(groups.Select(GroupViewData.Create).ToList());
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult CreateGroup(string name)
        {
            SchoolLocator.GroupService.AddGroup(name);
            return GroupsList();
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult EditGroupName(int groupId, string name)
        {
            SchoolLocator.GroupService.EditGroup(groupId, name);
            return GroupsList();
        }
        
        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult DeleteGroup(int groupId)
        {
            SchoolLocator.GroupService.DeleteGroup(groupId);
            return GroupsList();
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AssignStudentToGroup(int groupId, IntList studentIds)
        {
            SchoolLocator.GroupService.UpdateStudentGroups(groupId, studentIds);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult GroupExplorer()
        {
            var schools = SchoolLocator.SchoolService.GetSchools();
            var gradeLevels = SchoolLocator.GradeLevelService.GetGradeLevels();
            throw new NotImplementedException();
        }
    }
}