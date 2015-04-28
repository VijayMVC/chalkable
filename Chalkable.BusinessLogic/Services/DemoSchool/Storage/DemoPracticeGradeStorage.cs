using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPracticeGradeStorage:BaseDemoIntStorage<PracticeGrade>
    {
        public DemoPracticeGradeStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {

        }
    }
}
