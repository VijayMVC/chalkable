using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.SchoolsViewData;


namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolController : ChalkableController
    {
        
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(Guid districtId, int? start, int? count)
        {
            count = count ?? 10;
            start = start ?? 0;
            var schools = MasterLocator.SchoolService.GetSchools(districtId, start.Value, count.Value);
            return Json(schools.Transform(SchoolViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult UpdateStudyCenterEnabled(Guid? districtId, Guid? schoolId, DateTime? enabledTill)
        {
            MasterLocator.SchoolService.UpdateStudyCenterEnabled(districtId, schoolId, enabledTill);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult UpdateMessagingDisabled(Guid? districtId, Guid? schoolId, bool disabled)
        {
            MasterLocator.SchoolService.UpdateMessagingDisabled(districtId, schoolId, disabled);
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin, DistrictAdmin")]
        public ActionResult UpdateMessagingSettings(Guid? schoolId, bool studentMessaging, bool studentToClassOnly, bool teacherToStudentMessaging, bool teacherToClassOnly)
        {
            MasterLocator.SchoolService.UpdateMessagingSettings(Context.DistrictId, schoolId, studentMessaging, studentToClassOnly, teacherToStudentMessaging, teacherToClassOnly);
            return Json(true);
        }
        [AuthorizationFilter("SysAdmin")]
        public ActionResult UpdateAssessmentEnabled(Guid? districtId, Guid? schoolId, bool enabled)
        {
            MasterLocator.SchoolService.UpdateAssessmentEnabled(districtId, schoolId, enabled);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult SchoolPrograms()
        {
            var schoolPrograms = SchoolLocator.SchoolProgramService.GetAll();
            return Json(SchoolProgramViewData.Create(schoolPrograms));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult UserLocalSchools()
        {
            var schoolYears = SchoolLocator.SchoolYearService.GetSchoolYears().GroupBy(x => x.Id).Select(x => x.First());
            var userLocalSchools = SchoolLocator.SchoolService.GetSchoolsByIds(schoolYears.Select(x => x.Id).ToList());
            return Json(LocalSchoolViewData.Create(userLocalSchools));
        }

        /*[AuthorizationFilter("SysAdmin")]
        public ActionResult Summary(Guid schoolId)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            var school = MasterLocator.SchoolService.GetById(schoolId);
            return Json(SchoolInfoViewData.Create(school));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult People(Guid schoolId, int? roolId, Guid? gradeLevelId, int? start, int? count)
        {
            var school = MasterLocator.SchoolService.GetById(schoolId);
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(school.Id);
            var persons = SchoolLocator.PersonService.GetPersons();
            var studentsCount = persons.Count(x => x.RoleRef == CoreRoles.STUDENT_ROLE.Id);
            var teachersCount = persons.Count(x => x.RoleRef == CoreRoles.TEACHER_ROLE.Id);
            var adminsCount = persons.Count(x => x.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id)
                              + persons.Count(x => x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id)
                              + persons.Count(x => x.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id);
            var resView = SchoolPeopleViewData.Create(school, studentsCount, teachersCount, adminsCount);
            return Json(resView);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetPersons(Guid schoolId, int? roleId, IntList gradeLevelIds, int? start, int? count, bool? byLastName)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            var roleName = roleId.HasValue ? CoreRoles.GetById(roleId.Value).LoweredName : null;
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, null, roleName, null, gradeLevelIds));
        }*/
    }
}
