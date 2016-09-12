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
        public ActionResult GroupExplorer(int groupId)
        {
            var res = SchoolLocator.GroupService.GetGroupExplorerInfo(groupId);
            return Json(GroupExplorerViewData.Create(res));
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult GetStudentsForGroup(int groupId, int schoolYearId, int gradeLevelId, IntList classesIds, IntList coursesIds)
        {
            var students = SchoolLocator.GroupService.GetStudentsForGroup(groupId, schoolYearId, gradeLevelId, classesIds, coursesIds);
            return Json(StudentForGroupViewData.Create(students));
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
        public ActionResult AssignStudents(int groupId, IntList studentIds)
        {
            SchoolLocator.GroupService.AssignStudents(groupId, studentIds);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UnassignStudents(int groupId, IntList studentIds)
        {
            SchoolLocator.GroupService.UnssignStudents(groupId, studentIds);
            return Json(true);
        }


        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AssignSchoolGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            SchoolLocator.GroupService.AssignGradeLevel(groupId, schoolYearId, gradeLevelId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UnassignSchoolGradeLevel(int groupId, int schoolYearId, int gradeLevelId)
        {
            SchoolLocator.GroupService.UnssignGradeLevel(groupId, schoolYearId, gradeLevelId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AssignSchool(int groupId, int schoolYearId)
        {
            SchoolLocator.GroupService.AssignStudentsBySchoolYear(groupId, schoolYearId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UnassignSchool(int groupId, int schoolYearId)
        {
            SchoolLocator.GroupService.UnssignStudentsBySchoolYear(groupId, schoolYearId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult AssignAllShools(int groupId)
        {
            SchoolLocator.GroupService.AssignAll(groupId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin")]
        public ActionResult UnassignAllShools(int groupId)
        {
            SchoolLocator.GroupService.UnassignAll(groupId);
            return Json(true);
        }
    }
}