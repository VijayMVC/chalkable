using System;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
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
            SchoolLocator.PersonService.Edit(model.PersonId, model.Email, model.FirstName, model.LastName, model.Gender, model.Salutation, model.BirthdayDate);
            return EditPersonAdditionalInfo(model);
        }

        private Person EditPersonAdditionalInfo(AdminTeacherInputModel model)
        {
            SaveAddresses(model);
            SavePhones(model);
            var person = SchoolLocator.PersonService.GetPerson(model.PersonId);
            ReLogOn(person);
            return person;
        }

        protected void ReLogOn(Person person)
        {
            //TODO: think about how to get rememberMe  
            var user = MasterLocator.UserService.GetByLogin(person.Email);
            var context = LogOn(false, us => us.ReLogin(user.Id));
            if (context == null)
                throw new ChalkableSecurityException();
        }
        
        protected void SavePhones(AdminTeacherInputModel model)
        {
            var prev = SchoolLocator.PhoneService.GetPhones(model.PersonId);
            foreach (var phone in prev)
            {
                if (!model.Phones.Any(x=>x.Id == phone.Id))
                    SchoolLocator.PhoneService.Delete(phone.Id);
            }
            if (model.Phones != null)
                foreach (var phone in model.Phones)
                {
                    if (phone.Id.HasValue)
                        SchoolLocator.PhoneService.Edit(phone.Id.Value, phone.Value, (PhoneType)phone.Type, phone.IsPrimary);
                    else
                        throw new NotImplementedException();
                        //SchoolLocator.PhoneService.Add(model.PersonId, phone.Value, (PhoneType)phone.Type, phone.IsPrimary);
                }
        }

        protected void SaveAddresses(AdminTeacherInputModel model)
        {
            var prev = SchoolLocator.AddressService.GetAddress(model.PersonId);
            foreach (var address in prev)
            {
                if (!model.Addresses.Any(x => x.Id == address.Id))
                    SchoolLocator.AddressService.Delete(address.Id);
            }
            if (model.Addresses != null)
                foreach (var address in model.Addresses)
                {
                    throw new NotImplementedException();
                    //if (address.Id.HasValue)
                    //    SchoolLocator.AddressService.Edit(address.Id.Value, address.Value, string.Empty, (AddressType)address.Type);
                    //else
                    //    SchoolLocator.AddressService.Add(model.PersonId, address.Value, string.Empty, (AddressType)address.Type);
                }
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult GetPersons(int? roleId, IntList gradeLevelIds, int? start, int? count, bool? byLastName, string filter)
        {
            var roleName = roleId.HasValue ? CoreRoles.GetById(roleId.Value).Name : null;
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, byLastName, filter, roleName, null, gradeLevelIds));
        }
    }
}