using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.SisImportFacade;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;


namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolController : ChalkableController
    {
        //TODO: add
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(Guid? districtId, int? start, int? count, bool? demoOnly, bool? unimportedOnly)
        {
            count = count ?? 10;
            start = start ?? 0;
            var schools = MasterLocator.SchoolService.GetSchools(districtId, start.Value, count.Value);
            return Json(schools.Transform(SchoolViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Summary(Guid schoolId)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            var school = MasterLocator.SchoolService.GetById(schoolId);
            var sisData = MasterLocator.SchoolService.GetSyncData(school.Id);
            return Json(SchoolInfoViewData.Create(school, sisData));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult People(Guid schoolId, int? roolId, Guid? gradeLevelId, int? start, int? count)
        {
            var school = MasterLocator.SchoolService.GetById(schoolId);
            if(SchoolLocator.Context.SchoolId != schoolId)
                 SchoolLocator = MasterLocator.SchoolServiceLocator(school.Id);
            var persons = SchoolLocator.PersonService.GetPersons();
            var countsPerRole = persons.GroupBy(x => x.RoleRef).ToDictionary(x => x.Key, x => x.Count());
            var studentsCount = countsPerRole[CoreRoles.STUDENT_ROLE.Id];
            var teachersCount = countsPerRole[CoreRoles.TEACHER_ROLE.Id];
            var adminsCount = countsPerRole[CoreRoles.ADMIN_GRADE_ROLE.Id] 
                              + countsPerRole[CoreRoles.ADMIN_EDIT_ROLE.Id]
                              + countsPerRole[CoreRoles.ADMIN_VIEW_ROLE.Id];
            var sisData = MasterLocator.SchoolService.GetSyncData(schoolId);
            var resView = SchoolPeopleViewData.Create(school, sisData, studentsCount, teachersCount, adminsCount);
            return Json(resView);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetPersons(Guid schoolId, string roleName, GuidList gradeLevelIds, int? start, int? count, int? sortType)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, sortType, null, roleName, null, gradeLevelIds));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SisSyncInfo(Guid schoolId)
        {
            var syncInfo = MasterLocator.SchoolService.GetSyncData(schoolId);
            return Json(SisSyncViewData.Create(syncInfo));
        }

        public ActionResult ListTimeZones()
        {
            var tzCollection = DateTimeTools.GetAll();
            return Json(tzCollection);
        }

        public ActionResult GetSchoolsForImport(Guid districtId)
        {
            var distr = MasterLocator.DistrictService.GetById(districtId);
            var existing = MasterLocator.SchoolService.GetSchools(districtId);
            var importService = SisImportProvider.CreateImportService(distr.SisSystemType, Guid.Empty, 0, null, null);
            var res = importService.GetSchools();
            res = res.Where(x => !existing.Any(y => y.Name == x.Name)).ToList();
            return Json(res);
        }
    }
}
