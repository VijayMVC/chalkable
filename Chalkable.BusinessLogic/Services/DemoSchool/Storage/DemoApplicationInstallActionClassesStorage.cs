using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallActionClassesStorage:BaseDemoGuidStorage<ApplicationInstallActionClasses>
    {
        public DemoApplicationInstallActionClassesStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }

        
        public override void Setup()
        {
        }
    }
}
