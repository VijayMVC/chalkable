using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingScaleStorage: BaseDemoIntStorage<GradingScale>
    {
        public DemoGradingScaleStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        } 
    }
}
