using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoContactRelationshipStorage : BaseDemoIntStorage<ContactRelationship>
    {
        public DemoContactRelationshipStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }
    }

    public class DemoStudentContactStorage : BaseDemoIntStorage<StudentContact>
    {
        public DemoStudentContactStorage(DemoStorage storage) : base(storage, x=> x.ContactRef)

        {
        }
    }
}
