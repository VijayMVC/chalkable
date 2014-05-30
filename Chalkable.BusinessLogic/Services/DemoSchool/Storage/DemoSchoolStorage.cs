namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolStorage:BaseDemoIntStorage<Data.School.Model.School>
    {
        public DemoSchoolStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
    }
}
