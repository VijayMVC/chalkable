using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceReasonStorage
    {
        private Dictionary<int, AttendanceReason> attendanceReasonData = new Dictionary<int, AttendanceReason>(); 
        public void Add(IList<AttendanceReason> reasons)
        {
            foreach (var attendanceReason in reasons)
            {
                var reason = attendanceReasonData.FirstOrDefault(x => x.Key == attendanceReason.Id).Value;
                if (reason == null)
                {
                    attendanceReasonData.Add(attendanceReason.Id, attendanceReason);
                }
            }
        }

        public void Delete(int id)
        {
            attendanceReasonData.Remove(id);
        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public void Update(IList<AttendanceReason> reasons)
        {
            foreach (var attendanceReason in reasons)
            {
                var reason = attendanceReasonData.FirstOrDefault(x => x.Key == attendanceReason.Id).Value;
                if (reason != null)
                {
                    attendanceReasonData[attendanceReason.Id] =  attendanceReason;
                }
            }
        }

        public IList<AttendanceReason> GetAll()
        {
            return attendanceReasonData.Select(x => x.Value).ToList();
        }

        public AttendanceReason GetById(int id)
        {
            return attendanceReasonData.First(x => x.Key == id).Value;
        }
    }
}
