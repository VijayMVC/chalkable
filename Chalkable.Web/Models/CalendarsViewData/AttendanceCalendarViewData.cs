using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

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
        public int AttendanceType { get; set; }
        public int Count { get; set; }

        public static IList<AttendanceCalendarItemViewData> Create(IDictionary<AttendanceTypeEnum, int> attendanceCountDic)
        {
            return attendanceCountDic.Select(x => new AttendanceCalendarItemViewData
            {
                AttendanceType = (int)x.Key,
                Count = x.Value
            }).ToList();
        }
    }
    public class AttendanceForStudentCalendarItemViewData : AttendanceCalendarItemViewData
    {
        public Guid PersonId { get; set; }
        public string ClassName { get; set; }
        public Guid TeacherId { get; set; }
        public int PeriodOrder { get; set; }
        public Guid PeriodId { get; set; }

        public static IList<AttendanceForStudentCalendarItemViewData> Create(IList<ClassAttendanceDetails> attendances)
        {
            return attendances.Select(x => Create(x, 0)).ToList();
        }
        public static AttendanceForStudentCalendarItemViewData Create(ClassAttendanceDetails attendances, int count)
        {
            return new AttendanceForStudentCalendarItemViewData
            {
                PersonId = attendances.Student.Id,
                AttendanceType = (int)attendances.Type,
                ClassName = attendances.Class.Name,
                PeriodId = attendances.ClassPeriod.PeriodRef,
                PeriodOrder = attendances.ClassPeriod.Period.Order,
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
        public Guid PersonId { get; set; }

        protected AttendanceForStudentCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }


        public static AttendanceForStudentCalendarViewData Create(DateTime date, bool isCurrentMonth, Guid personId
            , IList<ClassAttendanceDetails> attendances)
        {
            attendances = attendances.Where(x => x.Date == date && x.Type != AttendanceTypeEnum.NotAssigned).ToList();

            var moreCount = 0;
            IList<AttendanceForStudentCalendarItemViewData> itemAttendances;
            var count = attendances.Count(x => x.Type != AttendanceTypeEnum.Present);
            var showGroupedData = count > NON_PRESENT_COUNT;
            if (showGroupedData)
            {
                itemAttendances = attendances
                    .GroupBy(x => x.Type)
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
                IsAbsent = attendances.All(x => x.Type == AttendanceTypeEnum.Absent),
                IsExcused = attendances.All(x => x.Type == AttendanceTypeEnum.Excused),
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
        public Guid ClassId { get; set; }
        protected AttendanceForClassCalendarViewData(DateTime date, bool isCurrentMonth)
            : base(date, isCurrentMonth)
        {
        }

        public static AttendanceForClassCalendarViewData Create(DateTime date, bool isCurrentMonth, Guid classId, IList<ClassAttendanceDetails> attendances)
        {
            var groupedAtt = attendances.Where(x=>x.Date == date).GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Count());
            return new AttendanceForClassCalendarViewData(date, isCurrentMonth)
                {
                    ClassId = classId,
                    Attendances = AttendanceCalendarItemViewData.Create(groupedAtt)
                };
        }

    }
}