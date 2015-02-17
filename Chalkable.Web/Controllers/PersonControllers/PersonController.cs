using System;
using System.Collections.Generic;
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

        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        //public ActionResult Schedule(int personId)
        //{
        //    var person = SchoolLocator.PersonService.GetPerson(personId);

        //}

        protected PersonScheduleViewData PrepareScheduleData(ShortPersonViewData person)
        {
            //var schoolYearId = GetCurrentSchoolYearId();
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolYearTime.Date);
            IList<ClassDetails> classes = new List<ClassDetails>();
            if(mp != null)
                classes = SchoolLocator.ClassService.GetClasses(mp.SchoolYearRef, mp.Id, person.Id);
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