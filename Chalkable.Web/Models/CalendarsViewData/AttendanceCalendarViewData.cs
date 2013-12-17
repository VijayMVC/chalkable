using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models.CalendarsViewData
{
    public class BaseAttendanceMonthCalendarViewData<TAttendanceData> : MonthCalendarViewData
    {
        public IList<TAttendanceData> Attendances { get; set; }
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

        public static IList<AttendanceForStudentCalendarItemViewData> Create(IList<ClassAttendanceDetails> attendances)
        {
            return attendances.Select(x => Create(x, 0)).ToList();
        }
        public static AttendanceForStudentCalendarItemViewData Create(ClassAttendanceDetails attendances, int count)
        {
            return new AttendanceForStudentCalendarItemViewData
            {
                PersonId = attendances.Student.Id,
                AttendanceType = attendances.Level,
                ClassName = attendances.Class.Name,
                //PeriodId = attendances.PeriodRef,//TODO: no data in INOW?
                //PeriodOrder = attendances.ClassPeriod.Period.Order,
                TeacherId = attendances.Class.TeacherRef,
                Count = count
            };
        }
    }
    
    public class AttendanceForStudentCalendarViewData : BaseAttendanceMonthCalendarViewData<AttendanceForStudentCalendarItemViewData>
    {
        public int MoreCount { get; set; }
        public const int NON_PRESENT_COUNT = 5;
        public const int ATTENDANCE_COUNT = 4;
        public bool IsExcused { get; set; }
        public bool IsAbsent { get; set; }
        public bool ShowGroupedData { get; set; }
        public int PersonId { get; set; }

        protected AttendanceForStudentCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }


        public static AttendanceForStudentCalendarViewData Create(DateTime date, bool isCurrentMonth, int personId
            , IList<ClassAttendanceDetails> attendances)
        {
            attendances = attendances.Where(x => x.Date == date && x.Level != null).ToList();

            var moreCount = 0;
            IList<AttendanceForStudentCalendarItemViewData> itemAttendances;
            var count = attendances.Count(x => x.IsAbsentOrLate);
            var showGroupedData = count > NON_PRESENT_COUNT;
            if (showGroupedData)
            {
                itemAttendances = attendances
                    .GroupBy(x => x.Level)
                    .ToDictionary(x => x.Key, x => x.ToList())
                    .OrderByDescending(x => x.Value.Count)
                    .Select(x => AttendanceForStudentCalendarItemViewData.Create(x.Value.First(), x.Value.Count)).ToList();
            }
            else itemAttendances = AttendanceForStudentCalendarItemViewData.Create(attendances);

            if (itemAttendances.Count > ATTENDANCE_COUNT)
            {
                moreCount = showGroupedData
                                ? itemAttendances.Skip(ATTENDANCE_COUNT).Sum(x => x.Count)
                                : itemAttendances.Skip(ATTENDANCE_COUNT).Count();

                itemAttendances = itemAttendances.Take(ATTENDANCE_COUNT).ToList();
            }
            var res = new AttendanceForStudentCalendarViewData(date, isCurrentMonth)
            {
                IsAbsent = attendances.Count > 0 && attendances.All(x => x.IsExcused),
                IsExcused = attendances.Count > 0 && attendances.All(x => x.IsExcused),
                Attendances = itemAttendances,
                MoreCount = moreCount,
                ShowGroupedData = showGroupedData,
                PersonId = personId
            };
            return res;
        }


    }

    public class AttendanceForClassCalendarViewData : BaseAttendanceMonthCalendarViewData<AttendanceCalendarItemViewData>
    {
        public int ClassId { get; set; }
        protected AttendanceForClassCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }

        public static AttendanceForClassCalendarViewData Create(DateTime date, bool isCurrentMonth, int classId, IList<ClassAttendanceDetails> attendances)
        {
            var groupedAtt = attendances.Where(x=>x.Date == date).GroupBy(x => x.Level).ToDictionary(x => x.Key, x => x.Count());
            return new AttendanceForClassCalendarViewData(date, isCurrentMonth)
                {
                    ClassId = classId,
                    Attendances = AttendanceCalendarItemViewData.Create(groupedAtt)
                };
        }

    }
}