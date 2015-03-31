using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
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

    public class DemoClassPeriodService : DemoSchoolServiceBase, IClassPeriodService
    {
        private DemoClassPeriodStorage ClassPeriodStorage { get; set; }
        public DemoClassPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ClassPeriodStorage = new DemoClassPeriodStorage();
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
            var dates = StorageLocator.DateStorage.GetDates(new DateQuery
            {
                FromDate = from,
                ToDate = to,
                SchoolYearId = schoolYearId,
                SchoolDaysOnly = true
            });
            var periods = StorageLocator.PeriodStorage.GetPeriods(schoolYearId);
            var classes = studentId.HasValue
                ? StorageLocator.ClassStorage.GetStudentClasses(schoolYearId, studentId.Value, null)
                : new List<ClassDetails>();

            if (teacherId.HasValue)
            {
                SER.ClassStorage.GetTeacherClasses(schoolYearId, teacherId.Value, null);
                classes = classes.Where(x => x.PrimaryTeacherRef == teacherId).ToList();
            }

            var classsPeriods = GetAll().Where(x => x.Period.SchoolYearRef == schoolYearId).ToList();
            var rooms = StorageLocator.RoomStorage.GetAll();
            var scheduleTimeSlots = StorageLocator.ScheduledTimeSlotStorage.GetAll();

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
