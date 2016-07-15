using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class PersonController : UserController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
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
            return Json(CurrentPersonViewData.Create(person, district, school, schoolYear, Context.Claims));
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

        protected T GetInfo<T>(int id, Func<PersonDetails, T> vdCreator) where T: PersonInfoViewData
        {
            if (!CanGetInfo(id))
                throw new ChalkableSecurityException(ChlkResources.ERR_VIEW_INFO_INVALID_RIGHTS);
            
            var person = SchoolLocator.PersonService.GetPersonDetails(id);
            var res = vdCreator(person);
            PrepareUserLoginData(res, person);
            return res;
        }

        private void PrepareUserLoginData(PersonInfoViewData personVD, Person person)
        {
            if(!person.UserId.HasValue) return;

            var user = MasterLocator.UserService.GetBySisUserId(person.UserId.Value, Context.DistrictId);
            if (MasterLocator.UserService.CanChangeUserLogin(user.Id))
            {
                personVD.Login = user.Login;
                personVD.CanEditLogin = true;
            }
        }

        protected virtual bool CanGetInfo(int personId)
        {
            return BaseSecurity.IsDistrictOrTeacher(SchoolLocator.Context) || SchoolLocator.Context.PersonId == personId;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GetPersons(int? start, int? count, string filter)
        {
            var res = SchoolLocator.PersonService.SearchPersons(filter, true, start ?? 0, count ?? 30);
            return Json(res.Transform(ShortPersonViewData.Create));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult UpdateInfo(int personId, string email)
        {
            Trace.Assert(email != null);
            string errorMessage;
            SchoolLocator.PersonService.EditEmailForCurrentUser(personId, email, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new ChalkableException(errorMessage));

            var res = GetInfo(personId, PersonInfoViewData.Create);
            return Json(res);
        }
    }
}