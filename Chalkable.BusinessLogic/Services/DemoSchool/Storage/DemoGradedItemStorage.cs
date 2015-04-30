using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradedItemStorage : BaseDemoIntStorage<GradedItem>
    {
        public DemoGradedItemStorage(DemoStorage storage): base(storage, x => x.Id)
        {
        }
    }
}
