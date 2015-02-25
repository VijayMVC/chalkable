using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingScaleRangeStorage: BaseDemoIntStorage<GradingScaleRange>
    {
        public DemoGradingScaleRangeStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }
    }
}
