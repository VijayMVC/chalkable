﻿using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
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

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST, true, CallType.Get, new[] { AppPermissionType.Discipline })]
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
        public ActionResult ClassList(DateTime? date, int classId, int? start, int? count)
        {
            //return FakeJson("~/fakeData/disciplineClassList.json");
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var currentDate = (date ?? SchoolLocator.Context.NowSchoolTime).Date;
            var disciplines = SchoolLocator.DisciplineService.GetClassDisciplineDetails(classId, currentDate);
            IList<DisciplineView> res = new List<DisciplineView>();
            if (disciplines != null)
            {
                res = DisciplineView.Create(disciplines, Context.UserLocalId ?? 0).ToList();   
            }
            res = res.OrderBy(x => x.Student.LastName).ThenBy(x => x.Student.FirstName).ToList();
            return Json(new PaginatedList<DisciplineView>(res, start.Value / count.Value, count.Value));
        }


        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher", Preference.API_DESCR_CLASS_DISCIPLINE_LIST_STUDENT_DISCIPLINE, true, CallType.Get, new[] { AppPermissionType.Discipline })]
        public ActionResult ListStudentDiscipline(int? schoolYearId, int schoolPersonId, DateTime? date)
        {
            return FakeJson("~/fakeData/studentDisciplines.json");
            //return Json(GetDisciplines(schoolYearId, schoolPersonId, null, date));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult SetClassDiscipline(ClassDiscipline discipline)
        {
            //foreach (var discInputModel in disciplineList.Disciplines)
            //{
            //    if (discInputModel.DisciplineTypeIds != null && discInputModel.DisciplineTypeIds.Count > 0)
            //    {
            //        ISet<int> discplineTypesSet = new HashSet<int>();
            //        foreach (var discplineTypeId in discInputModel.DisciplineTypeIds)
            //        {
            //            if (!discplineTypesSet.Contains(discplineTypeId))
            //                discplineTypesSet.Add(discplineTypeId);
            //        }
            //        SchoolLocator.DisciplineService.SetClassDiscipline(discInputModel.ClassPersonId, discInputModel.ClassPeriodId, discInputModel.Date,
            //                                                           discplineTypesSet, discInputModel.Description);
            //    }
            //    else
            //    {
            //        SchoolLocator.DisciplineService.DeleteClassDiscipline(discInputModel.ClassPersonId, discInputModel.ClassPeriodId, discInputModel.Date);
            //    }
            //}
            SchoolLocator.DisciplineService.SetClassDiscipline(discipline);
            return Json(true);
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