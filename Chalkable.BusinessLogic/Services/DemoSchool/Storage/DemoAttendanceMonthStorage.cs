using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceMonthStorage : BaseDemoIntStorage<AttendanceMonth>
    {
        public DemoAttendanceMonthStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
    }
}
