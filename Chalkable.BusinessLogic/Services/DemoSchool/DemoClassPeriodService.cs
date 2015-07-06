using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoClassPeriodStorage : BaseDemoIntStorage<ClassPeriod>
    {
        public DemoClassPeriodStorage()
            : base(null, true)
        {
        }
    }

    public class DemoScheduledTimeSlotStorage : BaseDemoGuidStorage<ScheduledTimeSlot>
    {
        public DemoScheduledTimeSlotStorage() : base(null, true)
        {
        }
    }

    public class DemoClassPeriodService : DemoSchoolServiceBase, IClassPeriodService
    {
        private DemoClassPeriodStorage ClassPeriodStorage { get; set; }
        private DemoScheduledTimeSlotStorage ScheduledTimeSlotStorage { get; set; }
        public DemoClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ClassPeriodStorage = new DemoClassPeriodStorage();
            ScheduledTimeSlotStorage = new DemoScheduledTimeSlotStorage();
        }
        
        public void Add(IList<ClassPeriod> classPeriods)
        {
            ClassPeriodStorage.Add(classPeriods);
        }

        public void Delete(IList<ClassPeriod> classPeriods)
        {
            throw new NotImplementedException();
        }

        public Class CurrentClassForTeacher(int personId, DateTime dateTime)
        {
            return ServiceLocator.ClassService.GetById(DemoSchoolConstants.AlgebraClassId);

        }

        public void AddScheduleTimeSlots(IList<ScheduledTimeSlot> slots)
        {
            ScheduledTimeSlotStorage.Add(slots);
        }

        private static ScheduleItem CreateScheduleItem(Date date, Period period, ScheduledTimeSlot scheduledTime, Class c, Room room)
        {
            var res = new ScheduleItem
            {
                PeriodId = period.Id,
                Day = date.Day,
                PeriodOrder = period.Order,
                IsSchoolDay = date.IsSchoolDay,
                SchoolYearId = date.SchoolYearRef,
                DayTypeId = date.DayTypeRef,
                PeriodName = period.Name
            };
            if (c != null)
            {
                res.ClassId = c.Id;
                res.ClassName = c.Name;
                res.ClassNumber = c.ClassNumber;
                res.ClassDescription = c.Description;
                res.ChalkableDepartmentId = c.ChalkableDepartmentRef;
            }
            if (room != null)
            {
                res.RoomId = room.Id;
                res.RoomNumber = room.RoomNumber;
            }
            if (scheduledTime != null)
            {
                res.StartTime = scheduledTime.StartTime;
                res.EndTime = scheduledTime.EndTime;
            }
            return res;
        }


        public IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime @from, DateTime to)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            return GetSchedule(syId, teacherId, studentId, classId, from, to);
        }

        public IList<ScheduleItem> GetSchedule(int schoolYearId, int? teacherId, int? studentId, int? classId, DateTime from, DateTime to)
        {
            var dates = ((DemoCalendarDateService)ServiceLocator.CalendarDateService).GetDates(new DateQuery
            {
                FromDate = from,
                ToDate = to,
                SchoolYearId = schoolYearId,
                SchoolDaysOnly = true
            });
            var periods = ServiceLocator.PeriodService.GetPeriods(schoolYearId);
            var classes = studentId.HasValue
                ? ServiceLocator.ClassService.GetStudentClasses(schoolYearId, studentId.Value, null)
                : new List<ClassDetails>();

            if (teacherId.HasValue)
            {
                ServiceLocator.ClassService.GetTeacherClasses(schoolYearId, teacherId.Value, null);
                classes = classes.Where(x => x.PrimaryTeacherRef == teacherId).ToList();
            }

            var classsPeriods = ClassPeriodStorage.GetAll().Where(x => x.Period.SchoolYearRef == schoolYearId).ToList();
            var rooms = ((DemoRoomService)ServiceLocator.RoomService).GetRooms();
            var scheduleTimeSlots = ScheduledTimeSlotStorage.GetAll();

            var res = new List<ScheduleItem>();
            foreach (var date in dates)
            {
                if (date.IsSchoolDay)
                    foreach (var period in periods)
                    {
                        var scheduleSlot = scheduleTimeSlots.FirstOrDefault(x => x.BellScheduleRef == date.BellScheduleRef && x.PeriodRef == period.Id);
                        var clsIds = classsPeriods.Where(x => x.PeriodRef == period.Id).Select(x => x.ClassRef);

                        var classesList = classes.Where(x => clsIds.Contains(x.Id));
                        foreach (var cls in classesList)
                        {
                            var room = rooms.FirstOrDefault(x => x.Id == cls.RoomRef);
                            res.Add(CreateScheduleItem(date, period, scheduleSlot, cls, room));
                        }

                    }
            }
            return res;
        }
    }
}
