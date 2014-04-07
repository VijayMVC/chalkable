using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPeriodStorage:BaseDemoStorage<int, ClassPeriod>
    {
        private int index = 0;
        public DemoClassPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(ClassPeriodQuery classPeriodQuery)
        {
            return GetClassPeriods(classPeriodQuery).Count > 0;
        }

        public void Add(ClassPeriod res)
        {
            data.Add(index++, res);
        }

        public void Add(IList<ClassPeriod> classPeriods)
        {
            foreach (var classPeriod in classPeriods)
            {
                Add(classPeriod);
            }
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

            return classPeriods.ToList();

            //
            //if (classPeriodQuery.SchoolYearId.HasValue)
            //    classPeriods = classPeriods.Where(x => x.)

            //
            //if (classPeriodQuery.Time.HasValue)
            //    classPeriods = classPeriods.Where(x => x.)
            //
            //if (classPeriodQuery.TeacherId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.StudentId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.RoomId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.MarkingPeriodId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)

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
            throw new System.NotImplementedException();
        }
    }
}
