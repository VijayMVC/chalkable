using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AttendancesViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class BaseAttendanceMonthCalendarViewData<TShortAttendanceData> : MonthCalendarViewData
    {
        public IList<TShortAttendanceData> Attendances { get; set; }
        protected BaseAttendanceMonthCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }
    }
    public class AttendanceCalendarItemViewData
    {
        public string AttendanceType { get; set; }
        public int Count { get; set; }

        public static IList<AttendanceCalendarItemViewData> Create(IDictionary<string, int> attendanceCountDic)
        {
            return attendanceCountDic.Select(x => new AttendanceCalendarItemViewData
            {
                AttendanceType = x.Key,
                Count = x.Value
            }).ToList();
        }
    }
    public class AttendanceForStudentCalendarItemViewData : AttendanceCalendarItemViewData
    {
        public int PersonId { get; set; }
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public int PeriodOrder { get; set; }
        public int PeriodId { get; set; }

        public static AttendanceForStudentCalendarItemViewData Create(StudentPeriodAttendance attendance, int count)
        {
            return new AttendanceForStudentCalendarItemViewData
            {
                PersonId = attendance.Student.Id,
                AttendanceType = attendance.Level,
                ClassName = attendance.Class.Name,
                PeriodId = attendance.Period.Id,
                PeriodOrder = attendance.Period.Order,
                TeacherId = attendance.Class.PrimaryTeacherRef,
                Count = count
            };
        }
    }
   
    public class AttendanceForStudentCalendarViewData : BaseAttendanceMonthCalendarViewData<AttendanceForStudentCalendarItemViewData>
    {
        public int MoreCount { get; set; }
        public const int NON_PRESENT_COUNT = 5;
        public const int ATTENDANCE_COUNT = 4;
        public bool ShowGroupedData { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsExcused { get; set; }
        public StudentDateAttendanceViewData StudentAttendance { get; set; }
        
        protected AttendanceForStudentCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }

        
        public static AttendanceForStudentCalendarViewData Create(DateTime date, bool isCurrentMonth, int personId, IList<StudentDateAttendance> studentAttendances, IList<AttendanceReason> reasons)
        {

            var moreCount = 0;
            IList<AttendanceForStudentCalendarItemViewData> itemAttendances;
            var studentAttendance = studentAttendances.FirstOrDefault(att => att.Date == date);
            var attendances = studentAttendance != null ? studentAttendance.StudentPeriodAttendances : new List<StudentPeriodAttendance>();
            var count = attendances.Count(x => x.IsAbsentOrLate);
            var showGroupedData = count > NON_PRESENT_COUNT;
            if (showGroupedData)
            {
                itemAttendances = attendances
                    .GroupBy(x => x.Level)
                    .ToDictionary(x => x.Key, x => x.ToList())
                    .OrderByDescending(x => x.Value.Count)
                    .Select(x => AttendanceForStudentCalendarItemViewData.Create(x.Value.First(), x.Value.Count))
                    .ToList();
            }
            else itemAttendances = attendances.Select(x=>AttendanceForStudentCalendarItemViewData.Create(x,1)).ToList();

            if (itemAttendances.Count > ATTENDANCE_COUNT)
            {
                moreCount = showGroupedData
                                ? itemAttendances.Skip(ATTENDANCE_COUNT).Sum(x => x.Count)
                                : itemAttendances.Skip(ATTENDANCE_COUNT).Count();

                itemAttendances = itemAttendances.Take(ATTENDANCE_COUNT).ToList();
            }
            var res = new AttendanceForStudentCalendarViewData(date, isCurrentMonth)
                {
                    IsAbsent = studentAttendance != null && studentAttendance.IsAbsent,
                    IsExcused = studentAttendance != null && studentAttendance.IsExcused,
                    Attendances = itemAttendances,
                    MoreCount = moreCount,
                    ShowGroupedData = showGroupedData,
                };
            if (studentAttendance != null)
                res.StudentAttendance = StudentDateAttendanceViewData.Create(studentAttendance, reasons);
            return res;
        }

    }

    public class AttendanceForClassCalendarViewData : MonthCalendarViewData
    {
        public int ClassId { get; set; }
        public bool HasAttendanceIssues { get; set; }
        protected AttendanceForClassCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }

        public static AttendanceForClassCalendarViewData Create(DateTime date, bool isCurrentMonth, int classId, IList<ClassPeriodAttendance> attendances)
        {
            var classAtt = attendances != null ? attendances.FirstOrDefault(a => a.Date == date) : null;
            return new AttendanceForClassCalendarViewData(date, isCurrentMonth)
                {
                    ClassId = classId,
                    HasAttendanceIssues = classAtt != null && classAtt.StudentAttendances.Count(x=>x.IsAbsentOrLate || x.IsExcused) > 0
                };
        }

    }
}