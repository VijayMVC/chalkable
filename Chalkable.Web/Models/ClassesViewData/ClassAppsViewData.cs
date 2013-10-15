using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassAppsViewData : ClassViewData
    {
        public AppsBudgetViewData AppsBudget { get; set; }
        private ClassAppsViewData(ClassDetails classDetails) : base(classDetails) { }

        public static ClassAppsViewData Create(ClassDetails classDetails, decimal? reserve, decimal? balance, IList<ApplicationInstall> installedApps, IList<Application> applications)
        {
            var res = new ClassAppsViewData(classDetails);
            IList<InstalledApplicationViewData> installedApplicationView = new List<InstalledApplicationViewData>();
            if (installedApps != null)
                installedApplicationView = InstalledApplicationViewData.Create(installedApps, null, applications);
            res.AppsBudget = AppsBudgetViewData.Create(balance, reserve, installedApplicationView);
            return res;
        }
    }
}