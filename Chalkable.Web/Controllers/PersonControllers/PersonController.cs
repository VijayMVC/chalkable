using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
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
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var person = SchoolLocator.PersonService.GetPerson(Context.PersonId.Value);
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
            if (MasterLocator.Context.PersonId == id)
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
                   || SchoolLocator.Context.PersonId == personId;
        }

        protected Person UpdateTeacherOrAdmin(AdminTeacherInputModel model)
        {
            throw new NotImplementedException();
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetPersons(IntList roleIds, IntList gradeLevelIds, int? start, int? count, bool? byLastName, string filter)
        {
            var res = new List<PersonViewData>();
            if (roleIds != null && roleIds.Count > 0)
            {
                foreach (var roleId in roleIds)
                {
                    var roleName = CoreRoles.GetById(roleId).Name;
                    res.AddRange(PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, roleName, null, gradeLevelIds));
                }
                res = byLastName.HasValue && byLastName.Value
                          ? res.OrderBy(x => x.LastName).ToList()
                          : res.OrderBy(x => x.FirstName).ToList();
            }
            else res = PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, null, null, gradeLevelIds);
            //var roleName = roleId.HasValue ? CoreRoles.GetById(roleId.Value).Name : null;
            return Json(res);
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
            return Json(PersonViewData.Create(res));
        }

    }
}