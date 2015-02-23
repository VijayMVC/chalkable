using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAutoGradeStorage:BaseDemoIntStorage<AutoGrade>
    {
        public DemoAutoGradeStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }


        public void SetAutoGrade(AutoGrade autograde)
        {
           var item = data.First(
                x => x.Value.AnnouncementApplicationRef == autograde.AnnouncementApplicationRef && autograde.StudentRef == x.Value.StudentRef);

            data[item.Key] = autograde;
        }
    }
}
