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

        protected PersonInfoViewData GetInfo(int id, Func<PersonDetails, PersonInfoViewData> vdCreator)
        {
            if (!CanGetInfo(id))
                throw new ChalkableSecurityException(ChlkResources.ERR_VIEW_INFO_INVALID_RIGHTS);
            
            var person = SchoolLocator.PersonService.GetPersonDetails(id);
            return vdCreator(person);
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
            if (personId != Context.PersonId)
                throw new ChalkableSecurityException("User can change email just for himself");
            Trace.Assert(email != null);
            string errorMessage;
            SchoolLocator.PersonService.EditEmailForCurrentUser(email, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new ChalkableException(errorMessage));

            var res = GetInfo(personId, PersonInfoViewData.Create);
            if(Context.Role == CoreRoles.TEACHER_ROLE)
                res.Email = MasterLocator.UserService.GetUserEmailById(Context.UserId);
            return Json(res);
        }
    }
}