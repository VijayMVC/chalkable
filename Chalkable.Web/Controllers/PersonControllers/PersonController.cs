﻿using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class PersonController : UserController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_USER_ME, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Me()
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var person = SchoolLocator.PersonService.GetPerson(Context.PersonId.Value);
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException("User has no valid school year id");
            if (!Context.DistrictId.HasValue || !Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException("User is not assigned to any school");
            return Json(CurrentPersonViewData.Create(person, Context.DistrictId.Value, Context.SchoolYearId.Value, Context.SchoolLocalId.Value));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var person = SchoolLocator.PersonService.GetPerson(personId);
            var schoolYearId = GetCurrentSchoolYearId();
            var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, null, personId);
            return Json(PersonScheduleViewData.Create(person, classes));
        }

        public ActionResult ReChangePassword(int id, string newPassword)
        {
            if (MasterLocator.Context.PersonId == id)
            {
                MasterLocator.UserService.ChangePassword(MasterLocator.Context.Login, newPassword);
                return Json(true);
            }
            throw new ChalkableException(ChlkResources.ERR_NOT_CURRENT_USER);
        }

        protected PersonInfoViewData GetInfo(int id, Func<PersonDetails, PersonInfoViewData> vdCreator)
        {
            if (!CanGetInfo(id))
                throw new ChalkableSecurityException(ChlkResources.ERR_VIEW_INFO_INVALID_RIGHTS);

            var person = SchoolLocator.PersonService.GetPersonDetails(id);
            return vdCreator(person);
        }

        private bool CanGetInfo(int personId)
        {
            return BaseSecurity.IsAdminOrTeacher(SchoolLocator.Context)
                   || SchoolLocator.Context.PersonId == personId;
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetPersons(int? start, int? count, string filter)
        {
            var res = SchoolLocator.PersonService.SearchPersons(filter, true, start ?? 0, count ?? 30);
            return Json(res.Transform(ShortPersonViewData.Create));
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult UpdateInfo(int personId, string email)
        {
            string errorMessage;
            SchoolLocator.PersonService.EditEmail(personId, email, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { data = errorMessage, success = false });
            //ReLogOn(res);
            var res = SchoolLocator.PersonService.GetPersonDetails(personId);
            return Json(PersonInfoViewData.Create(res));
        }

    }
}