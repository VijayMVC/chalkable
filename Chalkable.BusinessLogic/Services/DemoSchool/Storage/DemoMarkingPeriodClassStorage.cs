using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMarkingPeriodClassStorage:BaseDemoStorage<int, MarkingPeriodClass>
    {
        private int index = 0;
        public DemoMarkingPeriodClassStorage(DemoStorage storage) : base(storage)
        {
        }

        public MarkingPeriodClass GetMarkingPeriodClassOrNull(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            throw new NotImplementedException();
        }

        public void Delete(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            var mpcs = GetMarkingPeriodClasses(markingPeriodClassQuery);
            foreach (var markingPeriodClass in mpcs)
            {
                var item = data.First(x => x.Value == markingPeriodClass);
                data.Remove(item.Key);
            }
        }

        private IList<MarkingPeriodClass> GetMarkingPeriodClasses(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            var mpcs = data.Select(x => x.Value);


            if (markingPeriodClassQuery.ClassId.HasValue)
                mpcs = mpcs.Where(x => x.ClassRef == markingPeriodClassQuery.ClassId);
            if (markingPeriodClassQuery.MarkingPeriodId.HasValue)
                mpcs = mpcs.Where(x => x.MarkingPeriodRef == markingPeriodClassQuery.MarkingPeriodId);

            return mpcs.ToList();
        }

        public void Add(MarkingPeriodClass mpc)
        {
            data.Add(index++, mpc);
        }

        public void Add(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            foreach (var markingPeriodClass in markingPeriodClasses)
            {
                Add(markingPeriodClass);
            }
        }

        public bool Exists(int? classId, int? markingPeriodId)
        {
            var mpc = GetMarkingPeriodClasses(new MarkingPeriodClassQuery
            {
                ClassId = classId,
                MarkingPeriodId = markingPeriodId
            });

            return mpc.ToList().Count > 0;
        }

        public void Setup()
        {
           Add(new MarkingPeriodClass
           {
               SchoolRef = 1,
               MarkingPeriodRef = 1,
               ClassRef = 1
           });

           Add(new MarkingPeriodClass
           {
               SchoolRef = 1,
               MarkingPeriodRef = 1,
               ClassRef = 2
           });
        }
    }
}
