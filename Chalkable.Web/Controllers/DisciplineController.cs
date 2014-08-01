using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DisciplineController : ChalkableController
    {
        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult List(DateTime? date, int? start, int? count)
        {
            return FakeJson("~/fakeData/disciplineList.json");
            //start = start ?? 0;
            //count = count ?? DEFAULT_PAGE_SIZE;
            ////var res = GetDisciplines(null, null, classId, date);
            ////return Json(new PaginatedList<DisciplineView>(res, start.Value/count.Value, count.Value));
            //int currentYearId = GetCurrentSchoolYearId();
            //var datev = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            //var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentYearId, datev);
            //var list = StudentDisciplineSummaryViewData.Create(disciplines);
            //return Json(new PaginatedList<StudentDisciplineSummaryViewData>(list, start.Value / count.Value, count.Value));
        }

        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult Summary(DateTime? date, IntList gradeLevelIds)
        {
            return FakeJson("~/fakeData/adminDisciplines.json");
            //start = start ?? 0;
            //count = count ?? DEFAULT_PAGE_SIZE;
            ////var res = GetDisciplines(null, null, classId, date);
            ////return Json(new PaginatedList<DisciplineView>(res, start.Value/count.Value, count.Value));
            //int currentYearId = GetCurrentSchoolYearId();
            //var datev = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            //var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(currentYearId, datev);
            //var list = StudentDisciplineSummaryViewData.Create(disciplines);
            //return Json(new PaginatedList<StudentDisciplineSummaryViewData>(list, start.Value / count.Value, count.Value));
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ClassList(DateTime? date, int classId, int? start, int? count, bool? byLastName)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolYearTime).Date;
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(classId, currentDate, null);
            IList<DisciplineView> res = new List<DisciplineView>();
            if (disciplines != null)
            {
                res = DisciplineView.Create(disciplines, Context.UserLocalId ?? 0).ToList();   
            }
            if (byLastName.HasValue && byLastName.Value)
                res = res.OrderBy(x => x.Student.LastName).ThenBy(x => x.Student.FirstName).ToList();
            else
                res = res.OrderBy(x => x.Student.FirstName).ThenBy(x => x.Student.LastName).ToList();
            return Json(new PaginatedList<DisciplineView>(res, start.Value / count.Value, count.Value));
        }


        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher")]
        public ActionResult ListStudentDiscipline(int? schoolYearId, int schoolPersonId, DateTime? date)
        {
            return FakeJson("~/fakeData/studentDisciplines.json");
            //return Json(GetDisciplines(schoolYearId, schoolPersonId, null, date));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetClassDiscipline(ClassDisciplineInputModel discipline)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            IList<Infraction> infractions = null;
            if (discipline.InfractionsIds != null && discipline.InfractionsIds.Count > 0)
            {
                infractions = SchoolLocator.InfractionService.GetInfractions();
                infractions = infractions.Where(x => discipline.InfractionsIds.Contains(x.Id)).ToList();
            }
            var classDisciplineModel = new ClassDiscipline
                {
                    ClassId = discipline.ClassId,
                    Date = discipline.Date,
                    Id = discipline.Id,
                    Description = discipline.Description,
                    Infractions = infractions ?? new List<Infraction>(),
                    StudentId = discipline.StudentId
                };
            var res = SchoolLocator.DisciplineService.SetClassDiscipline(classDisciplineModel);
            return Json(DisciplineView.Create(res, Context.UserLocalId.Value));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_CLASS_DISCIPLINE_STUDENT_DISCIPLINE_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Discipline })]
        public ActionResult StudentDisciplineSummary(int personId, int markingPeriodId)
        {
            return FakeJson("~/fakeData/studentDisciplinesSummary.json");
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var currentDateTime = Context.NowSchoolTime.Date;
            //var student = SchoolLocator.PersonService.GetPersonDetails(personId);
            //var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            //var query = new ClassDisciplineQuery
            //{
            //    MarkingPeriodId = mp.Id,
            //    PersonId = student.Id,
            //    FromDate = mp.StartDate,
            //    ToDate = currentDateTime,
            //};
            //var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(query);
            //var res = StudentDisciplineDetailedViewData.Create(student, disciplines, mp);
            //return Json(res, 6);
        }
    }
}