using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoContactRelationshipStorage : BaseDemoIntStorage<ContactRelationship>
    {
        public DemoContactRelationshipStorage(DemoStorage storage)
            : base(storage, x => x.Id, false)
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
