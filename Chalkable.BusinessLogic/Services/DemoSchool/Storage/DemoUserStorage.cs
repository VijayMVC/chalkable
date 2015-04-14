using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoUserStorage : BaseDemoGuidStorage<User>
    {
        public DemoUserStorage(DemoStorage storage) 
            : base(storage, x => x.Id, false)
        {
        }
    }
}
