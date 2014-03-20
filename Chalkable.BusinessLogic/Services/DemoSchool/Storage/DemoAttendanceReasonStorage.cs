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
            throw new NotImplementedException();
            foreach (var attendanceReason in reasons)
            {
                var reason = attendanceReasonData.FirstOrDefault(x => x.Key == attendanceReason.Id).Value;
                if (reason == null)
                {
                    attendanceReasonData.Add(attendanceReason.Id, attendanceReason);
                }
            }
        }

        public void Update(IList<AttendanceReason> reasons)
        {
            throw new NotImplementedException();
            foreach (var attendanceReason in reasons)
            {
                var reason = attendanceReasonData.FirstOrDefault(x => x.Key == attendanceReason.Id).Value;
                if (reason != null)
                {
                    attendanceReasonData[attendanceReason.Id] =  attendanceReason;
                }
            }
        }
     
    }
}
