﻿using System;
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
            return FakeJson("~/fakeData/appBudget.json");
            //if (!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //var schoolId = Context.SchoolId.Value;
            //var toSchoolPayment = (double)MasterLocator.FundService.GetToSchoolPayment(schoolId);
            //var paymentforApp = (double)MasterLocator.FundService.GetPaymentForApps(schoolId);
            //return Json(BudgetBalanceViewData.Craete(toSchoolPayment, paymentforApp));
        }

        [AuthorizationFilter("Teacher, Student, AdminGrade, AdminEdit, AdminView")]
        public ActionResult GetPersonBudgetBalance(int personId)
        {
            return Json(new {balance = 0});
        }

        [AuthorizationFilter("Teacher, Student")]
        public ActionResult PersonFunds()
        {
            return FakeJson("~/fakeData/personFunds.json");
        }

    }
}