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
    public class DemoApplicationInstallActionStorage:BaseDemoStorage<int, ApplicationInstallAction>
    {
        public DemoApplicationInstallActionStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallAction> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallAction appInstall)
        {
            if (!data.ContainsKey(appInstall.Id))
                data[appInstall.Id] = appInstall;
        }

        public ApplicationInstallAction GetLastAppInstallAction(Guid id, int value)
        {
            throw new NotImplementedException();
        }

        public void Update(ApplicationInstallAction res)
        {
            throw new NotImplementedException();
        }
    }
}
