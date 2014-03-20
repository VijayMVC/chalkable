using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceReasonStorage:BaseDemoStorage<int ,AttendanceReason>
    {
        public DemoAttendanceReasonStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(IList<AttendanceReason> reasons)
        {
            foreach (var attendanceReason in reasons.Where(attendanceReason => !data.ContainsKey(attendanceReason.Id)))
            {
                data.Add(attendanceReason.Id, attendanceReason);
            }
        }

        public void Update(IList<AttendanceReason> reasons)
        {
            foreach (var attendanceReason in reasons)
            {
                if (data.ContainsKey(attendanceReason.Id))
                {
                    data[attendanceReason.Id] =  attendanceReason;
                }
            }
        }
     
    }
}
