using System.Collections.Generic;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionRoleStorage:BaseDemoStorage<int, ApplicationInstallActionRole>
    {
        public DemoApplicationInstallActionRoleStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstallActionRole> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstallActionRole appInstall)
        {
            var id = GetNextFreeId();
            appInstall.Id = id;
            data[appInstall.Id] = appInstall;
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
