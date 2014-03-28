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


        public void Setup()
        {
            var reasons = new List<AttendanceReason>();
            reasons.Add(new AttendanceReason
            {
                Id = 1,
                Code = "IL",
                Name = "Illness",
                Description = "",
                Category = "E",
                IsSystem = false,
                IsActive = true
            });
            reasons.Add(new AttendanceReason
            {
                Id = 2,
                Code = "SA",
                Name = "School Activity",
                Description = "",
                Category = "E",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 3,
                Code = "FRE",
                Name = "Family Reason(Excused)",
                Description = "",
                Category = "E",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 4,
                Code = "FRU",
                Name = "Family Reason(Unexcused)",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 5,
                Code = "OSS",
                Name = "Out of School Suspension",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 6,
                Code = "ISS",
                Name = "In-School Suspension",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 7,
                Code = "SK",
                Name = "Skipping",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 8,
                Code = "DD",
                Name = "Doctor or Dentist",
                Description = "",
                Category = "E",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 9,
                Code = "IM",
                Name = "Noncompliance",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 9,
                Code = "MB",
                Name = "Missed Bus",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });

            reasons.Add(new AttendanceReason
            {
                Id = 10,
                Code = "LB",
                Name = "Late Bus",
                Description = "",
                Category = "E",
                IsSystem = false,
                IsActive = false
            });

            reasons.Add(new AttendanceReason
            {
                Id = 10,
                Code = "EC",
                Name = "Early Checkout",
                Description = "",
                Category = "U",
                IsSystem = false,
                IsActive = true
            });


            Add(reasons);
        }
     
    }
}
