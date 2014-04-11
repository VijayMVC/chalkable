using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web.Hosting;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionDepartmentStorage:BaseDemoStorage<int, ApplicationInstallActionDepartment>
    {
        public DemoApplicationInstallActionDepartmentStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallActionDepartment> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallActionDepartment appInstall)
        {
            var id = GetNextFreeId();
            appInstall.Id = id;
            data[appInstall.Id] = appInstall;
        }
    }
}
