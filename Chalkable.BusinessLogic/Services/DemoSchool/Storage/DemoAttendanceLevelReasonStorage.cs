using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceLevelReasonStorage : BaseDemoIntStorage<AttendanceLevelReason>
    {
        public DemoAttendanceLevelReasonStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        }

        public IList<AttendanceLevelReason> GetForAttendanceReason(int attendanceReasonId)
        {
            return data.Where(x => x.Value.AttendanceReasonRef == attendanceReasonId).Select(x => x.Value).ToList();
        }
    }
}
