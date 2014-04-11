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
    public class DemoApplicationInstallActionGradeLevelStorage:BaseDemoStorage<int, ApplicationInstallActionGradeLevel>
    {
        public DemoApplicationInstallActionGradeLevelStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallActionGradeLevel> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallActionGradeLevel appInstall)
        {
            var id = GetNextFreeId();
            appInstall.Id = id;
            data[appInstall.Id] = appInstall;
        }
    }
}
