using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult List(DateTime? date, int? start, int? count)
        {
            Guid currentYearId = GetCurrentSchoolYearId();
            var datev = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentYearId, datev);
            var list = StudentDisciplineSummaryViewData.Create(disciplines);
            start = start ?? 0;
            count = count ?? DEFAULT_PAGE_SIZE;
            return Json(new PaginatedList<StudentDisciplineSummaryViewData>(list, start.Value / count.Value, count.Value));
        }

        [RequireRequestValue("schoolYearId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ListStudentDiscipline(Guid? schoolYearId, Guid schoolPersonId, DateTime? date)
        {
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var schoolYearIdv = schoolYearId ?? GetCurrentSchoolYearId();
            var list = SchoolLocator.DisciplineService.GetClassDisciplineDetails(schoolYearIdv, schoolPersonId, currentDate, currentDate, true);
            var canEdit = BaseSecurity.IsAdminGrader(SchoolLocator.Context);
            var classes = SchoolLocator.Context.Role == CoreRoles.TEACHER_ROLE ? SchoolLocator.ClassService.GetClasses(schoolYearIdv, null, null) : null;
            return Json(DisciplineView.Create(list, classes, canEdit));
        }
    }
}