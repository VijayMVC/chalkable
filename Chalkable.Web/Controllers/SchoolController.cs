using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.SchoolsViewData;


namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolController : ChalkableController
    {
        
        [AuthorizationFilter("SysAdmin, DistrictAdmin")]
        public ActionResult List(Guid districtId, int? start, int? count)
        {
            count = count ?? 10;
            start = start ?? 0;
            var schools = MasterLocator.SchoolService.GetSchools(districtId, start.Value, count.Value);
            return Json(schools.Transform(SchoolViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new []{ AppPermissionType.User })]
        public ActionResult LocalSchools()
        {  
            var schools = SchoolLocator.SchoolService.GetSchools();
            return Json(LocalSchoolViewData.Create(schools));
        }


        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult UserLocalSchools()
        {
            return Json(LocalSchoolViewData.Create(SchoolLocator.SchoolService.GetUserLocalSchools()));
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
    }
}
