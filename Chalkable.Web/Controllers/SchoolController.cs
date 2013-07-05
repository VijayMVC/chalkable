using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.PersonViewDatas;


namespace Chalkable.Web.Controllers
{
    public class SchoolController : ChalkableController
    {
        [AuthorizationFilter("System Admin")]
        public ActionResult List(int? start, int? count, bool? demoOnly)
        {
            count = count ?? 10;
            start = start ?? 0;
            var schools = MasterLocator.SchoolService.GetSchools(start.Value, count.Value);
            return Json(schools.Transform(SchoolViewData.Create));
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult Summary(Guid schoolId)
        {
            var school = MasterLocator.SchoolService.GetById(schoolId);
            var sisData = MasterLocator.SchoolService.GetSyncData(school.Id);
            return Json(SchoolInfoViewData.Create(school, sisData));
        }

        [AuthorizationFilter("System Admin")]
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
            var resView = SchoolPeapleViewData.Create(school, sisData, studentsCount, teachersCount, adminsCount);
            return Json(resView);
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult GetPersons(Guid schoolId, int? roolId, GuidList gradeLevelIds, int? start, int? count, int? sortType)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            var sortTypeEnum = (SortTypeEnum?)sortType ?? SortTypeEnum.ByLastName;
            start = start ?? 0;
            count = count ?? 10;
            var res = SchoolLocator.PersonService.GetPersons(roolId, gradeLevelIds, null, sortTypeEnum, start.Value, count.Value);
            return Json(res.Transform(PersonViewData.Create));
        }
    }
}
