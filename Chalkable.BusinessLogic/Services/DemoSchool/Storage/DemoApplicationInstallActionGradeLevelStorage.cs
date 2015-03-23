using System;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionGradeLevelStorage:BaseDemoIntStorage<ApplicationInstallActionGradeLevel>
    {
        public DemoApplicationInstallActionGradeLevelStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }
    }
}
