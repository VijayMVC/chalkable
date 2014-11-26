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

        public bool Exists(ClassPeriodQuery classPeriodQuery)
        {
            return GetClassPeriods(classPeriodQuery).Count > 0;
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery classPeriodQuery)
        {

            var classPeriods = data.Select(x => x.Value);

            if (classPeriodQuery.PeriodId.HasValue)
                classPeriods = classPeriods.Where(x => x.PeriodRef == classPeriodQuery.PeriodId);

            if (classPeriodQuery.DateTypeId.HasValue)
                classPeriods = classPeriods.Where(x => x.DayTypeRef == classPeriodQuery.DateTypeId);



            if (classPeriodQuery.ClassIds != null)
                classPeriods = classPeriods.Where(x => classPeriodQuery.ClassIds.Contains(x.ClassRef));

            if (classPeriodQuery.Time.HasValue)
            {
                throw new NotImplementedException();
                /*classPeriods =
                    classPeriods.Where(
                        x =>
                            x.Period.StartTime <= classPeriodQuery.Time.Value &&
                            x.Period.EndTime >= classPeriodQuery.Time.Value);*/
            }

            if (classPeriodQuery.SchoolYearId.HasValue)
            {
                classPeriods =
                    classPeriods.Where(
                        x =>
                            x.Period.SchoolYearRef == classPeriodQuery.SchoolYearId);
            }

            if (classPeriodQuery.RoomId.HasValue)
            {
                var enumerable = classPeriods as IList<ClassPeriod> ?? classPeriods.ToList();
                var clsIds =
                    enumerable.Select(x => Storage.ClassStorage.GetById(x.ClassRef))
                        .Where(x => x.RoomRef == classPeriodQuery.RoomId)
                        .Select(x => x.Id);

                classPeriods = enumerable.Where(x => clsIds.Contains(x.ClassRef));
            }


            if (classPeriodQuery.StudentId.HasValue)
            {
                var csp = Storage.ClassPersonStorage.GetClassPerson(new ClassPersonQuery
                {
                    PersonId = classPeriodQuery.StudentId
                });

                classPeriods = classPeriods.Where(x => x.ClassRef == csp.ClassRef);
            }

            if (classPeriodQuery.TeacherId.HasValue)
            {
                var csp = Storage.ClassStorage.GetClassesComplex(new ClassQuery
                {
                    PersonId = classPeriodQuery.TeacherId
                }).Classes.Select(x => x.Id);

                classPeriods = classPeriods.Where(x => csp.Contains(x.ClassRef));
            }
            return classPeriods.ToList();
        }

        public IList<Class> GetAvailableClasses(int periodId)
        {
            var classIds = GetClassPeriods(new ClassPeriodQuery
            {
                PeriodId = periodId
            }).Select(x => x.ClassRef);

            return classIds.Select(classId => Storage.ClassStorage.GetById(classId)).ToList();
        }

        public IList<Room> GetAvailableRooms(int periodId)
        {
            var classIds = GetClassPeriods(new ClassPeriodQuery
            {
                PeriodId = periodId
            }).Select(x => x.ClassRef);


            var rooms = new List<Room>();

            foreach (var classId in classIds)
            {
                var cls = Storage.ClassStorage.GetById(classId);
                if (cls.RoomRef.HasValue)
                    rooms.Add(Storage.RoomStorage.GetById(cls.RoomRef.Value));

            }

            return rooms;
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
