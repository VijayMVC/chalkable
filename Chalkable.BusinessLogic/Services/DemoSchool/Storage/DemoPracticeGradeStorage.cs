using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPracticeGradeStorage:BaseDemoIntStorage<PracticeGrade>
    {
        public DemoPracticeGradeStorage()
            : base(x => x.Id, true)
        {

        }
    }
}
