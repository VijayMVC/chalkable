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
            foreach (var attendanceReason in reasons)
            {
                attendanceReason.Id = GetNextFreeId();
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


        public override void Setup()
        {
            var reasons = new List<AttendanceReason>
            {
                new AttendanceReason
                {
                    Code = "IL",
                    Name = "Illness",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "SA",
                    Name = "School Activity",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "FRE",
                    Name = "Family Reason(Excused)",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "FRU",
                    Name = "Family Reason(Unexcused)",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "OSS",
                    Name = "Out of School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "ISS",
                    Name = "In-School Suspension",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "SK",
                    Name = "Skipping",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "DD",
                    Name = "Doctor or Dentist",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "IM",
                    Name = "Noncompliance",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "MB",
                    Name = "Missed Bus",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                },
                new AttendanceReason
                {
                    Code = "LB",
                    Name = "Late Bus",
                    Description = "",
                    Category = "E",
                    IsSystem = false,
                    IsActive = false
                },
                new AttendanceReason
                {
                    Code = "EC",
                    Name = "Early Checkout",
                    Description = "",
                    Category = "U",
                    IsSystem = false,
                    IsActive = true
                }
            };


            Add(reasons);
        }
     
    }
}
