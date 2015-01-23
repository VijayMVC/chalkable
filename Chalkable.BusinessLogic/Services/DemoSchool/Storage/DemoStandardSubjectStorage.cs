using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardSubjectStorage:BaseDemoIntStorage<StandardSubject>
    {
        public DemoStandardSubjectStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
    }
}
