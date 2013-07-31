using System;
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
    public class PersonController : ChalkableController
    {
        protected PersonInfoViewData GetInfo(Guid id, Func<PersonDetails, PersonInfoViewData> vdCreator)
        {
            if (!CanGetInfo(id))
                throw new ChalkableSecurityException(ChlkResources.ERR_VIEW_INFO_INVALID_RIGHTS);

            var person = SchoolLocator.PersonService.GetPersonDetails(id);
            return vdCreator(person);
        }

        private bool CanGetInfo(Guid personId)
        {
            return BaseSecurity.IsAdminOrTeacher(SchoolLocator.Context) 
                   || SchoolLocator.Context.UserId == personId;
        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_USER_ME, true, CallType.Get, new[] { AppPermissionType.User, })]
        public ActionResult Me()
        {
            var person = SchoolLocator.PersonService.GetPerson(SchoolLocator.Context.UserId);
            return Json(PersonViewData.Create(person));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Apps(Guid personId)
        {
            var person = SchoolLocator.PersonService.GetPerson(personId);
            var appsInstalls = SchoolLocator.AppMarketService.ListInstalledAppInstalls(personId);
            var instaledApps = SchoolLocator.AppMarketService.ListInstalled(personId, true);
            var balance = personId == SchoolLocator.Context.UserId
                              ? MasterLocator.FundService.GetUserBalance(personId, false)
                              : MasterLocator.FundService.GetUserBalance(personId);
            decimal? reserve = null;
            if (BaseSecurity.IsAdminViewer(Context) && Context.SchoolId.HasValue)
                reserve = MasterLocator.FundService.GetSchoolReserve(Context.SchoolId.Value);
            
            var res = PersonAppsViewData.Create(person, reserve, balance, instaledApps, appsInstalls);
            return Json(res, 5);
        }


        private const string addressIdFmt = "address-{0}-id";
        private const string addressDelFmt = "address-{0}-delete";
        private const string addressTypeFmt = "address-{0}-type";
        private const string addressValueFmt = "address-{0}-value";
        private const string addressNoteFmt = "address-{0}-note";
        protected void SaveAddresses(Guid personId, IntList addressIndexes)
        {
            if (addressIndexes != null)
            {
                foreach (var addressIndex in addressIndexes)
                {
                    Guid addressId = Guid.Parse(Request[string.Format(addressIdFmt, addressIndex)] ?? "0");
                    bool deleted = bool.Parse(Request[string.Format(addressDelFmt, addressIndex)] ?? "false");
                    if (deleted)
                    {
                        SchoolLocator.AddressService.Delete(addressId);
                        continue;
                    }

                    var type = (AddressType)int.Parse(Request[string.Format(addressTypeFmt, addressIndex)]);
                    string value = Request[string.Format(addressValueFmt, addressIndex)];
                    string note = Request[string.Format(addressNoteFmt, addressIndex)];
                    if (addressId == Guid.Empty)
                        SchoolLocator.AddressService.Edit(addressId, value, note, type);
                    else
                        SchoolLocator.AddressService.Add(personId, value, note, type);
                }
            }
        }

        private const string phoneIdFmt = "phone-{0}-id";
        private const string phoneIdDelFmt = "phone-{0}-delete";
        private const string phoneTypeFmt = "phone-{0}-type";
        private const string phoneValueFmt = "phone-{0}-value";
        private const string phoneIsPrimaryFmt = "phone-{0}-isprimary";
        protected void SavePhones(Guid personId, IntList phoneIndexes)
        {
            if (phoneIndexes != null)
            {
                foreach (var phoneIndex in phoneIndexes)
                {
                    Guid phoneId = Guid.Parse(Request[string.Format(phoneIdFmt, phoneIndex)] ?? "0");
                    bool deleted = Request.Params[string.Format(phoneIdDelFmt, phoneIndex)] == "on";

                    if (deleted)
                    {
                        SchoolLocator.PhoneService.Delete(phoneId);
                        continue;
                    }
                    var type = (PhoneType)int.Parse(Request[string.Format(phoneTypeFmt, phoneIndex)] ?? "0");
                    string value = Request[string.Format(phoneValueFmt, phoneIndex)];
                    bool isPrimary = bool.Parse(Request[string.Format(phoneIsPrimaryFmt, phoneIndex)] ?? "false");

                    if (phoneId == Guid.Empty)
                        SchoolLocator.PhoneService.Edit(phoneId, value, type, isPrimary);
                    else
                        SchoolLocator.PhoneService.Add(personId, value, type, isPrimary);
                }
            }
        }
  

        //TODO: reSignIn person  
        //protected void ReSignIn(Person person)
        //{
            
        //}

        private Person EditPersonAdditionalInfo(Guid personId, IntList addressIndexes, IntList phoneIndexes)
        {
            SaveAddresses(personId, addressIndexes);
            SavePhones(personId, phoneIndexes);
            var teacher = SchoolLocator.PersonService.GetPerson(personId);
            //ReSignIn(teacher);
            return teacher;
        }

        protected Person UpdateTeacherOrAdmin(Guid personId, string email, string firstName, string lastName
            , string gender, DateTime? birthdayDate,  string salutation, IntList addressIndexes, IntList phoneIndexes)
        {
            SchoolLocator.PersonService.Edit(personId, email, firstName, lastName, gender, salutation, birthdayDate);
            return EditPersonAdditionalInfo(personId, addressIndexes, phoneIndexes);
        }
    }
}