using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class AdminFeedViewData
    {
        //public IList<AttendanceListView> Attendance { get; set; }
        //public IList<DisciplineListView> DisciplineList { get; set; }
        public IList<DepartmentGradingStatViewData> GradingStats { get; set; }
        public AdminAttendanceBoxViewData NotInClassBox { get; set; }
        public AdminAttendanceBoxViewData AbsentForDay { get; set; }
        public AdminAttendanceBoxViewData AdbsentForMp { get; set; }


        public static AdminFeedViewData Create(IList<DepartmentGradingStatViewData> gradingStats
            , IDictionary<DateTime, int> stAbsentForDayStats, DateTime currentDate)
        {
            var res = new AdminFeedViewData { GradingStats = gradingStats };
            if (stAbsentForDayStats != null)
            {
                res.AbsentForDay = PrepareAbsentForDay(stAbsentForDayStats, currentDate);
                res.AdbsentForMp = PrepareAbsentForMp(stAbsentForDayStats);
            }
            return res;
        }
        private const string DAY_FORMAT = "ddd";
        private static AdminAttendanceBoxViewData PrepareAbsentForDay(IDictionary<DateTime, int> stAbsentForDayStats, DateTime currentDate)
        {
            var days = stAbsentForDayStats.Keys.Where(x => x < currentDate).OrderByDescending(x => x.Date).Take(6).ToList();
            var res = new AdminAttendanceBoxViewData {Stat = new List<StudentAttendanceStatViewData>()};
            foreach (var d in days)
            {
                res.Stat.Add(new StudentAttendanceStatViewData
                    {
                        StundetnCount = stAbsentForDayStats[d],
                        Summary = d.ToString(DAY_FORMAT)
                    });
            }
            if(stAbsentForDayStats.ContainsKey(currentDate))
                 res.DisplayNumber = stAbsentForDayStats[currentDate];
            return res;
        }
        
        private const string MONTH_FORMAT = "MMM";
        private static AdminAttendanceBoxViewData PrepareAbsentForMp(IDictionary<DateTime, int> stCountAbsentFromDay)
        {
            var res = new AdminAttendanceBoxViewData { Stat = new List<StudentAttendanceStatViewData>() };
            if(stCountAbsentFromDay.Count > 0)
                res.DisplayNumber = (int)stCountAbsentFromDay.Average(x => x.Value);
            var toDate = stCountAbsentFromDay.First().Key;
            var startDate = stCountAbsentFromDay.Last().Key;
            var currentMonth = 0;
            while (startDate < toDate)
            {
                if (stCountAbsentFromDay.ContainsKey(startDate))
                {
                    var summary = "";
                    if (currentMonth != startDate.Month)
                    {
                        currentMonth = startDate.Month;
                        summary = startDate.ToString(MONTH_FORMAT);
                    }
                    res.Stat.Add(new StudentAttendanceStatViewData
                    {
                        StundetnCount = stCountAbsentFromDay[startDate],
                        Summary = summary,
                    });
                }
                startDate = startDate.AddDays(1).Date;
            }
            return res;
        }
    }


    public class AdminAttendanceBoxViewData
    {
        public int? DisplayNumber { get; set; }
        public IList<StudentAttendanceStatViewData> Stat { get; set; }
    }

    public class StudentAttendanceStatViewData
    {
        public int StundetnCount { get; set; }
        public string Summary { get; set; }
    }
}