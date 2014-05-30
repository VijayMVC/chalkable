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
    public class DemoApplicationInstallActionDepartmentStorage:BaseDemoIntStorage<ApplicationInstallActionDepartment>
    {
        public DemoApplicationInstallActionDepartmentStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }
    }
}
