using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAutoGradeStorage:BaseDemoIntStorage<AutoGrade>
    {
        public DemoAutoGradeStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        public void SetAutoGrade(int announcementApplicationId, AutoGrade autograde)
        {
            data[announcementApplicationId] = autograde;
        }
    }
}
