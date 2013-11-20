using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common.Exceptions;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class FundController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView")]
        public ActionResult GetAppBudgetBalance()
        {
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var schoolId = Context.SchoolId.Value;
            var toSchoolPayment = (double)MasterLocator.FundService.GetToSchoolPayment(schoolId);
            var paymentforApp = (double)MasterLocator.FundService.GetPaymentForApps(schoolId);
            return Json(BudgetBalanceViewData.Craete(toSchoolPayment, paymentforApp));
        }
    }
}