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
    public class DemoApplicationInstallActionClassesStorage:BaseDemoStorage<Guid, ApplicationInstallActionClasses>
    {
        public DemoApplicationInstallActionClassesStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallActionClasses> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallActionClasses appInstall)
        {
            var id = Guid.NewGuid();
            appInstall.Id = id;
            data.Add(appInstall.Id, appInstall);
        }
    }
}
