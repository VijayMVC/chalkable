using System;
using System.Collections.Generic;
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
                if (!data.ContainsKey(attendanceLevelReason.Id))
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

        public void Setup()
        {
            var attendanceLevelReasons = new List<AttendanceLevelReason>
            {
                new AttendanceLevelReason
                {
                    Id = 56,
                    Level = "AO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 57,
                    Level = "AO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 58,
                    Level = "HO",
                    AttendanceReasonRef = 6,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 59,
                    Level = "HO",
                    AttendanceReasonRef = 2,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 60,
                    Level = "A",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 61,
                    Level = "A",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 62,
                    Level = "A",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 63,
                    Level = "A",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 64,
                    Level = "A",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 65,
                    Level = "A",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 66,
                    Level = "A",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 67,
                    Level = "H",
                    AttendanceReasonRef = 8,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 68,
                    Level = "H",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 69,
                    Level = "H",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 70,
                    Level = "H",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 71,
                    Level = "H",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 72,
                    Level = "H",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 73,
                    Level = "H",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 74,
                    Level = "H",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 75,
                    Level = "H",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 102,
                    Level = "IO",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 103,
                    Level = "IO",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 104,
                    Level = "IO",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 105,
                    Level = "IO",
                    AttendanceReasonRef = 1,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 106,
                    Level = "IO",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 107,
                    Level = "IO",
                    AttendanceReasonRef = 10,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 108,
                    Level = "IO",
                    AttendanceReasonRef = 9,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 109,
                    Level = "IO",
                    AttendanceReasonRef = 5,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 110,
                    Level = "IO",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 111,
                    Level = "IO",
                    AttendanceReasonRef = 12,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 120,
                    Level = "T",
                    AttendanceReasonRef = 8,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 121,
                    Level = "T",
                    AttendanceReasonRef = 3,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 122,
                    Level = "T",
                    AttendanceReasonRef = 4,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 123,
                    Level = "T",
                    AttendanceReasonRef = 1,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 124,
                    Level = "T",
                    AttendanceReasonRef = 11,
                    IsDefault = false
                },
                new AttendanceLevelReason
                {
                    Id = 125,
                    Level = "T",
                    AttendanceReasonRef = 10,
                    IsDefault = true
                },
                new AttendanceLevelReason
                {
                    Id = 126,
                    Level = "T",
                    AttendanceReasonRef = 7,
                    IsDefault = false
                }
            };


            Add(attendanceLevelReasons);
        }
    }
}
