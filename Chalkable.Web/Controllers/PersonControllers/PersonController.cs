using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class PersonController : UserController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_USER_ME, true, CallType.Get, new[] { AppPermissionType.User })]
        public ActionResult Me()
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            var person = SchoolLocator.PersonService.GetPerson(Context.UserLocalId.Value);
            return Json(PersonViewData.Create(person));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Apps(int personId)
        {
            throw new NotImplementedException();
            //var person = SchoolLocator.PersonService.GetPerson(personId);
            //var appsInstalls = SchoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
            //var instaledApps = SchoolLocator.AppMarketService.ListInstalled(personId, true);
            //var balance = personId == SchoolLocator.Context.UserLocalId
            //                  ? MasterLocator.FundService.GetUserBalance(personId, false)
            //                  : MasterLocator.FundService.GetUserBalance(personId);
            //decimal? reserve = null;
            //if (BaseSecurity.IsAdminViewer(Context) && Context.SchoolId.HasValue)
            //    reserve = MasterLocator.FundService.GetSchoolReserve(Context.SchoolId.Value);
            
            //var res = PersonAppsViewData.Create(person, reserve, balance, instaledApps, appsInstalls);
            //return Json(res, 5);
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var person = SchoolLocator.PersonService.GetPerson(personId);
            var schoolYearId = GetCurrentSchoolYearId();
            var classes = SchoolLocator.ClassService.GetClasses(schoolYearId, null, personId);
            return Json(PersonScheduleViewData.Create(person, classes));
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, Teacher")]
        public ActionResult UploadPicture(int personId)
        {
            throw new NotImplementedException();
           // return UploadPicture(MasterLocator.PersonPictureService, personId, null, null);
        }

        public ActionResult ReChangePassword(int id, string newPassword)
        {
            if (MasterLocator.Context.UserLocalId == id)
            {
                MasterLocator.UserService.ChangePassword(MasterLocator.Context.Login, newPassword);
                //MixPanelService.ChangedPassword(ServiceLocator.Context.UserName);
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
                   || SchoolLocator.Context.UserLocalId == personId;
        }

        protected Person UpdateTeacherOrAdmin(AdminTeacherInputModel model)
        {
            throw new NotImplementedException();
        }

        private Person EditPersonAdditionalInfo(AdminTeacherInputModel model)
        {
            throw new NotImplementedException();
        }

        protected void ReLogOn(Person person)
        {
            //TODO: think about how to get rememberMe  
            var user = MasterLocator.UserService.GetByLogin(person.Email);
            var context = LogOn(false, us => us.ReLogin(user.Id));
            if (context == null)
                throw new ChalkableSecurityException();
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetPersons(int? roleId, IntList gradeLevelIds, int? start, int? count, bool? byLastName, string filter)
        {
            var roleName = roleId.HasValue ? CoreRoles.GetById(roleId.Value).Name : null;
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, roleName, null, gradeLevelIds));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult UpdateInfo(int personId, string email)
        {
            var res = SchoolLocator.PersonService.EditEmail(personId, email);
            if (res == null)
                return Json(new { data = "There is user with that email in Chalkable", success = false });
            return Json(PersonViewData.Create(res));
        }
    }
}