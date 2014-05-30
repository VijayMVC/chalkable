using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolOptionStorage:BaseDemoIntStorage<SchoolOption>
    {
        public DemoSchoolOptionStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }
    }
}
