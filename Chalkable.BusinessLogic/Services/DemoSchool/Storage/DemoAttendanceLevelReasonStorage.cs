using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceLevelReasonStorage : BaseDemoStorage<int, AttendanceLevelReason>
    {
        public DemoAttendanceLevelReasonStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            foreach (var attendanceLevelReason in attendanceLevelReasons)
            {
                attendanceLevelReason.Id = GetNextFreeId();
                data[attendanceLevelReason.Id] = attendanceLevelReason;
            }
        }

        public void Update(List<AttendanceLevelReason> attendanceLevelReasons)
        {
            foreach (var attendanceLevelReason in attendanceLevelReasons)
            {
                if (data.ContainsKey(attendanceLevelReason.Id))
                    data[attendanceLevelReason.Id] = attendanceLevelReason;
            }
        }

        public IList<AttendanceLevelReason> GetForAttendanceReason(int attendanceReasonId)
        {
            return data.Where(x => x.Value.AttendanceReasonRef == attendanceReasonId).Select(x => x.Value).ToList();
        } 

        public override void Setup()
        {
            var attendanceLevelReasons = new List<AttendanceLevelReason>
            {
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "AO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "HO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "A",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 8,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "H",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "IO",
                    AttendanceReasonRef = 12,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 10,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Level = "T",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                }
            };


            Add(attendanceLevelReasons);
        }
    }
}
