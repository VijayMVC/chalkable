using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
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
            return Json(SchoolViewData.Create(school));
        }

        [AuthorizationFilter("System Admin")]
        public ActionResult People(Guid schoolId, int? roolId, Guid? gradeLevelId, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
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
            if(roolId.HasValue)
               persons = persons.Where(x => x.RoleRef == roolId).ToList();
            if (gradeLevelId.HasValue)
                persons = persons.Where(x => x.StudentInfo != null && x.StudentInfo.GradeLevelRef == gradeLevelId).ToList();
            int totalCount = persons.Count;
            persons = persons.Skip(start.Value).Take(count.Value).ToList();
            var people = new PaginatedList<PersonViewData>(PersonViewData.Create(persons), start.Value/count.Value, count.Value, totalCount);
            var resView = SchoolPeapleViewData.Create(school, studentsCount, teachersCount, adminsCount, people);
            return Json(resView);
        }
    }
}
