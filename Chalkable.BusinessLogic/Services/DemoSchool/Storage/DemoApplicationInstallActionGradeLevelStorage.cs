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
    public class DemoApplicationInstallActionGradeLevelStorage:BaseDemoIntStorage<ApplicationInstallActionGradeLevel>
    {
        public DemoApplicationInstallActionGradeLevelStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
