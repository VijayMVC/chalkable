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
    }
}