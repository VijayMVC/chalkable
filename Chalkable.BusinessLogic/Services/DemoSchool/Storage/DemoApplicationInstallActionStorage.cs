using System;
using System.Linq;
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
    }
}
