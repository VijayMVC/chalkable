using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class AppsBudgetViewData
    {
        public IList<InstalledApplicationViewData> InstalledApplication { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Reserve { get; set; }

        public static AppsBudgetViewData Create(decimal? balance, decimal? reserve,
                                                IList<InstalledApplicationViewData> installedApplication)
        {
            return new AppsBudgetViewData
                {
                    Balance = balance,
                    Reserve = reserve,
                    InstalledApplication = installedApplication
                };
        }
    }
}