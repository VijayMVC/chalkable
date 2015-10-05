using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class AttendanceMonthViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public static IList<AttendanceMonthViewData> Create(IList<AttendanceMonth> attendanceMonths)
        {
            return attendanceMonths.Select(x => new AttendanceMonthViewData
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToList();
        }
    }
}