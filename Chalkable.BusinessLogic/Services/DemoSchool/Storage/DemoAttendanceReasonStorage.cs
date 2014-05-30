using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceReasonStorage:BaseDemoIntStorage<AttendanceReason>
    {
        public DemoAttendanceReasonStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
     
    }
}
