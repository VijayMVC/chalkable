using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAttendanceReasonStorage:BaseDemoIntStorage<AttendanceReason>
    {
        public DemoAttendanceReasonStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public override void Setup()
        {
            var reasons = new List<AttendanceReason>
            {
                new AttendanceReason
                {
                    Id = 1,
                    Code = "IL",
                    Name = "Illness",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(1)
                },
                new AttendanceReason
                {
                    Id = 2,
                    Code = "SA",
                    Name = "School Activity",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(2)
                },
                new AttendanceReason
                {
                    Id = 3,
                    Code = "FRE",
                    Name = "Family Reason(Excused)",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(3)
                },
                new AttendanceReason
                {
                    Id = 4,
                    Code = "FRU",
                    Name = "Family Reason(Unexcused)",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(4)
                },
                new AttendanceReason
                {
                    Id = 5,
                    Code = "OSS",
                    Name = "Out of School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(5)
                },
                new AttendanceReason
                {
                    Id = 6,
                    Code = "ISS",
                    Name = "In-School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(6)
                },
                new AttendanceReason
                {
                    Id = 7,
                    Code = "SK",
                    Name = "Skipping",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(7)
                },
                new AttendanceReason
                {
                    Id = 8,
                    Code = "DD",
                    Name = "Doctor or Dentist",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(8)
                },
                new AttendanceReason
                {
                    Id = 9,
                    Code = "IM",
                    Name = "Noncompliance",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(9)
                },
                new AttendanceReason
                {
                    Id = 10,
                    Code = "MB",
                    Name = "Missed Bus",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(10)
                },
                new AttendanceReason
                {
                    Id = 11,
                    Code = "LB",
                    Name = "Late Bus",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = false,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(11)
                },
                new AttendanceReason
                {
                    Id = 12,
                    Code = "EC",
                    Name = "Early Checkout",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true,
                    AttendanceLevelReasons = Storage.AttendanceLevelReasonStorage.GetForAttendanceReason(12)
                }
            };

            Add(reasons);
        }
     
    }
}
