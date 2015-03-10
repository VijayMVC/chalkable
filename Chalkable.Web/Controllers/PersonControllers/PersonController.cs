﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var person = SchoolLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException("User has no valid school year id");
            if (!Context.DistrictId.HasValue || !Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException("User is not assigned to any school");

            var district = MasterLocator.DistrictService.GetByIdOrNull(Context.DistrictId.Value);
            var school = MasterLocator.SchoolService.GetById(Context.DistrictId.Value, Context.SchoolLocalId.Value);
            var schoolYear = SchoolLocator.SchoolYearService.GetSchoolYearById(Context.SchoolYearId.Value);
            return Json(CurrentPersonViewData.Create(person, district, school, schoolYear));
        }

        protected PersonScheduleViewData PrepareScheduleData(ShortPersonViewData person)
        {
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolYearTime.Date);
            IList<ClassDetails> classes = new List<ClassDetails>();
            if (mp != null)
            {
                if (person.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                    classes = SchoolLocator.ClassService.GetTeacherClasses(mp.SchoolYearRef, person.Id, mp.Id);
                else if (person.Role.Id == CoreRoles.STUDENT_ROLE.Id)
                    classes = SchoolLocator.ClassService.GetStudentClasses(mp.SchoolYearRef, person.Id, mp.Id);
                else
                    throw new NotImplementedException();
            }
                
            return PersonScheduleViewData.Create(person, classes);
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
            var res = vdCreator(person);
            if (Context.PersonId == person.Id) //just for current user
            {
                res.Email = MasterLocator.UserService.GetUserEmailById(Context.UserId);
            }
            return res;
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
            if (personId != Context.PersonId)
                throw new ChalkableSecurityException("User can change email just for himself");
            Trace.Assert(email != null);
            string errorMessage;
            SchoolLocator.PersonService.EditEmailForCurrentUser(email, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { data = errorMessage, success = false });

            var res = GetInfo(personId, PersonInfoViewData.Create);
            return Json(res);
        }
    }
}