﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class BudgetBalanceViewData
    {
        public double StartSchoolBalance { get; set; }
        public double CurrentBalance { get; set; }
        public int PersentSpent { get; set; }

        public static BudgetBalanceViewData Craete(double startSchoolBalance, double paymentForApp)
        {
            var res = new BudgetBalanceViewData
                {
                    StartSchoolBalance = startSchoolBalance,
                    CurrentBalance = startSchoolBalance - paymentForApp
                };
            if (startSchoolBalance > 0 && paymentForApp > 0)
                res.PersentSpent = res.CurrentBalance > 0 ? (int)(100 * ((double)(paymentForApp / startSchoolBalance))) : 999;
            return res;
        }
    }
}