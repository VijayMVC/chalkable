using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    public class AdminController : PersonController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Info(Guid personId)
        {
            var res = GetInfo(personId, TeacherInfoViewData.Create);
            return Json(res);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult UpdateInfo(Guid personId, IntList addressIndexes, IntList phoneIndexes, string email,
            string gender, DateTime? birthdayDate, string salutation, string firstName, string lastName)
        {
            var teacher = UpdateTeacherOrAdmin(personId, email, firstName, lastName, gender, birthdayDate
                                               , salutation, addressIndexes, phoneIndexes);
            //MixPanelService.ChangedEmail(SchoolLocator.Context., email);
            return Json(GetInfo(teacher.Id, TeacherInfoViewData.Create));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetPersons(string roleName, GuidList gradeLevelIds, int? start, int? count, int? sortType)
        {
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, sortType, null, roleName, null, gradeLevelIds));
        }
    }
}