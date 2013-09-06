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

        //[RequireRequestValue("schoolYearId")]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ListStudentDiscipline(Guid? schoolYearId, Guid schoolPersonId, DateTime? date)
        {
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var schoolYearIdv = schoolYearId ?? GetCurrentSchoolYearId();
            var list = SchoolLocator.DisciplineService.GetClassDisciplineDetails(schoolYearIdv, schoolPersonId, currentDate, currentDate, true);
            var canEdit = BaseSecurity.IsAdminEditor(SchoolLocator.Context);
            return Json(DisciplineView.Create(list, Context.UserId, canEdit));
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetClassDiscipline(DisciplineListInputModel disciplineList)
        {
            foreach (var discInputModel in disciplineList.Disciplines)
            {
                if (discInputModel.DisciplineTypeIds != null && discInputModel.DisciplineTypeIds.Count > 0)
                {
                    ISet<Guid> discplineTypesSet = new HashSet<Guid>();
                    foreach (var discplineTypeId in discInputModel.DisciplineTypeIds)
                    {
                        if (!discplineTypesSet.Contains(discplineTypeId))
                            discplineTypesSet.Add(discplineTypeId);
                    }
                    SchoolLocator.DisciplineService.SetClassDiscipline(discInputModel.ClassPersonId, discInputModel.ClassPeriodId, discInputModel.Date,
                                                                       discplineTypesSet, discInputModel.Description);
                }
                else
                {
                    SchoolLocator.DisciplineService.DeleteClassDiscipline(discInputModel.ClassPersonId, discInputModel.ClassPeriodId, discInputModel.Date);
                }
            }
            return Json(true);
        }
    }
}