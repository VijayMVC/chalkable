namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolStorage:BaseDemoIntStorage<Data.School.Model.School>
    {
        public DemoSchoolStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public override void Setup()
        {
            Add(new Data.School.Model.School
            {
                Id = DemoSchoolConstants.SchoolId,
                IsActive = true,
                IsPrivate = true,
                Name = "SMITH"
            });
        }
    }
}
