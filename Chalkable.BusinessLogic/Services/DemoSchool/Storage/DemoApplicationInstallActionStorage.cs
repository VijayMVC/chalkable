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
    public class DemoApplicationInstallActionStorage:BaseDemoIntStorage<ApplicationInstallAction>
    {
        public DemoApplicationInstallActionStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }

        public ApplicationInstallAction GetLastAppInstallAction(Guid id, int userId)
        {
            return data.OrderByDescending(x => x.Value.Id).First(x => x.Value.ApplicationRef == id && x.Value.OwnerRef == userId).Value;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
