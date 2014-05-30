using System.Collections.Generic;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionRoleStorage:BaseDemoIntStorage<ApplicationInstallActionRole>
    {
        public DemoApplicationInstallActionRoleStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }
    }
}
