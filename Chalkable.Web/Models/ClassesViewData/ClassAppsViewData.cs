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
        public IList<InstalledApplicationViewData> InstalledApplication { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Reserve { get; set; }

        private ClassAppsViewData(ClassDetails classDetails) : base(classDetails) { }

        public static ClassAppsViewData Create(ClassDetails classDetails, decimal? reserve, decimal? balance, IList<ApplicationInstall> installedApps, IList<Application> applications)
        {
            var res = new ClassAppsViewData(classDetails) {Balance = balance, Reserve = reserve};
            if (installedApps != null)
                res.InstalledApplication = InstalledApplicationViewData.Create(installedApps, null, applications);
            return res;
        }
    }
}