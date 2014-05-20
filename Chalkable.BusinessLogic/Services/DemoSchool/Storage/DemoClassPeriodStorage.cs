using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery classPeriodQuery)
        {

            var classPeriods = data.Select(x => x.Value);

            if (classPeriodQuery.PeriodId.HasValue)
                classPeriods = classPeriods.Where(x => x.PeriodRef == classPeriodQuery.PeriodId);

            if (classPeriodQuery.DateTypeId.HasValue)
                classPeriods = classPeriods.Where(x => x.DayTypeRef == classPeriodQuery.DateTypeId);




            classPeriods = classPeriods.Where(x => classPeriodQuery.ClassIds.Contains(x.ClassRef));

            if (classPeriodQuery.Time.HasValue)
            {
                classPeriods =
                    classPeriods.Where(
                        x =>
                            x.Period.StartTime <= classPeriodQuery.Time.Value &&
                            x.Period.EndTime >= classPeriodQuery.Time.Value);
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

        public override void Setup()
        {
            Add(new ClassPeriod
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                DayTypeRef = 19,
                PeriodRef = DemoSchoolConstants.FirstPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = Storage.PeriodStorage.GetById(DemoSchoolConstants.FirstPeriodId)
            });

            Add(new ClassPeriod
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                DayTypeRef = 19,
                PeriodRef = DemoSchoolConstants.SecondPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = Storage.PeriodStorage.GetById(DemoSchoolConstants.SecondPeriodId)
            });

            Add(new ClassPeriod
            {
                ClassRef = DemoSchoolConstants.AlgebraClassId,
                DayTypeRef = 20,
                PeriodRef = DemoSchoolConstants.FirstPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = Storage.PeriodStorage.GetById(DemoSchoolConstants.FirstPeriodId)
            });

            Add(new ClassPeriod
            {
                ClassRef = DemoSchoolConstants.GeometryClassId,
                DayTypeRef = 20,
                PeriodRef = DemoSchoolConstants.SecondPeriodId,
                SchoolRef = DemoSchoolConstants.SchoolId,
                Period = Storage.PeriodStorage.GetById(DemoSchoolConstants.SecondPeriodId)
            });
        }
    }
}
