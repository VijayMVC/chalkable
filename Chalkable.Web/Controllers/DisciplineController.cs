using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult List(DateTime? date, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? DEFAULT_PAGE_SIZE;
            //var res = GetDisciplines(null, null, classId, date);
            //return Json(new PaginatedList<DisciplineView>(res, start.Value/count.Value, count.Value));
            int currentYearId = GetCurrentSchoolYearId();
            var datev = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentYearId, datev);
            var list = StudentDisciplineSummaryViewData.Create(disciplines);
            return Json(new PaginatedList<StudentDisciplineSummaryViewData>(list, start.Value / count.Value, count.Value));
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ClassList(DateTime? date, int classId, int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var teacherId = SchoolLocator.Context.Role == CoreRoles.TEACHER_ROLE ? SchoolLocator.Context.UserLocalId : default(int?);
            var cps = SchoolLocator.ClassPeriodService.GetClassPeriods(currentDate, classId, null, null, teacherId);
            var cp = cps.OrderBy(x => x.Period.StartTime).LastOrDefault();
            var res = new List<DisciplineView>();
            if (cp != null)
            {
                res = GetDisciplines(null, null, classId, currentDate);   
            }
            res = res.OrderBy(x => x.Student.LastName).ThenBy(x=>x.Student.FirstName).ToList();
            return Json(new PaginatedList<DisciplineView>(res, start.Value / count.Value, count.Value));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ListStudentDiscipline(int? schoolYearId, int schoolPersonId, DateTime? date)
        {
            return Json(GetDisciplines(schoolYearId, schoolPersonId, null, date));
        }

        private List<DisciplineView> GetDisciplines(int? schoolYearId, int? studentId, int? classId, DateTime? date)
        {
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(currentDate);
            if (mp == null)
                throw new NoMarkingPeriodException();
            
            var schoolYearIdv = schoolYearId ?? mp.SchoolYearRef;
            var list = SchoolLocator.DisciplineService.GetClassDisciplineDetails(new ClassDisciplineQuery
            {
                ClassId = classId,
                SchoolYearId = schoolYearIdv,
                FromDate = currentDate,
                ToDate = currentDate,
                NeedAllData = true,
                PersonId = studentId,
            });
            var canEdit = BaseSecurity.IsAdminEditor(SchoolLocator.Context);
            return DisciplineView.Create(list, Context.UserLocalId ?? 0, canEdit).ToList();
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetClassDiscipline(DisciplineListInputModel disciplineList)
        {
            foreach (var discInputModel in disciplineList.Disciplines)
            {
                if (discInputModel.DisciplineTypeIds != null && discInputModel.DisciplineTypeIds.Count > 0)
                {
                    ISet<int> discplineTypesSet = new HashSet<int>();
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


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_CLASS_DISCIPLINE_STUDENT_DISCIPLINE_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Discipline })]
        public ActionResult StudentDisciplineSummary(int personId, int markingPeriodId)
        {
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var currentDateTime = Context.NowSchoolTime.Date;
            var student = SchoolLocator.PersonService.GetPersonDetails(personId);
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var query = new ClassDisciplineQuery
            {
                MarkingPeriodId = mp.Id,
                PersonId = student.Id,
                FromDate = mp.StartDate,
                ToDate = currentDateTime,
            };
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(query);
            var res = StudentDisciplineDetailedViewData.Create(student, disciplines, mp);
            return Json(res, 6);
        }
    }
}