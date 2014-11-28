using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPeriodStorage:BaseDemoIntStorage<ClassPeriod>
    {
        public DemoClassPeriodStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            
            throw new NotImplementedException();
        }
        
        public IList<ScheduleItem> GetSchedule(int schoolYearId, int? teacherId, int? studentId, int? classId, DateTime from, DateTime to)
        {
            var dates = Storage.DateStorage.GetDates(new DateQuery {FromDate = from, ToDate = to, SchoolYearId = schoolYearId});
            var periods = Storage.PeriodStorage.GetPeriods(schoolYearId);
            var classes = Storage.ClassStorage.GetClassesComplex(new ClassQuery
                {
                    ClassId = classId,
                    PersonId = studentId,
                    SchoolYearId = schoolYearId
                }).Classes;
            if (teacherId.HasValue)
                classes = classes.Where(x => x.PrimaryTeacherRef == teacherId).ToList();
            var classsPeriods = GetAll().Where(x =>x.Period.SchoolYearRef == schoolYearId).ToList();
            var rooms = Storage.RoomStorage.GetAll();
            var scheduleTimeSlots = Storage.ScheduledTimeSlotStorage.GetAll();
            
            var res = new List<ScheduleItem>();
            foreach (var date in dates)
            {
                foreach (var period in periods)
                {
                    var scheduleSlot = scheduleTimeSlots.FirstOrDefault(x => x.BellScheduleRef == date.BellScheduleRef && x.PeriodRef == period.Id);
                    if (classsPeriods.Count > 0)
                    {
                        foreach (var classsPeriod in classsPeriods)
                        {
                            var c = classes.FirstOrDefault(x => x.Id == classsPeriod.ClassRef);
                            Room room = null;
                            if (c != null)
                                room = rooms.FirstOrDefault(x => x.Id == c.RoomRef);
                            res.Add(CreateScheduleItem(date, period, scheduleSlot, c, room));
                        }       
                    }
                    else res.Add(CreateScheduleItem(date, period, scheduleSlot, null, null));
                }
            }
            return res;
        }

        private ScheduleItem CreateScheduleItem(Date date, Period period, ScheduledTimeSlot scheduledTime, Class c, Room room)
        {
            var res = new ScheduleItem
                {
                    PeriodId = period.Id,
                    Day = date.Day,
                    PeriodOrder = period.Order,
                    IsSchoolDay = date.IsSchoolDay,
                    SchoolYearId = date.SchoolYearRef,
                    DayTypeId = date.DayTypeRef
                };
            if (c != null)
            {
                res.ClassId = c.Id;
                res.ClassName = c.Name;
                res.ClassNumber = c.ClassNumber;
                res.ClassDescription = c.Description;
                res.ChalkableDepartmentId = c.ChalkableDepartmentRef;
                res.GradeLevelId = c.GradeLevelRef;
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
    }
}
