﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMarkingPeriodClassStorage:BaseDemoIntStorage<MarkingPeriodClass>
    {
        public DemoMarkingPeriodClassStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public MarkingPeriodClass GetMarkingPeriodClassOrNull(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            return GetMarkingPeriodClasses(markingPeriodClassQuery).FirstOrDefault();
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

        private IEnumerable<MarkingPeriodClass> GetMarkingPeriodClasses(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            var mpcs = data.Select(x => x.Value);


            if (markingPeriodClassQuery.ClassId.HasValue)
                mpcs = mpcs.Where(x => x.ClassRef == markingPeriodClassQuery.ClassId);
            if (markingPeriodClassQuery.MarkingPeriodId.HasValue)
                mpcs = mpcs.Where(x => x.MarkingPeriodRef == markingPeriodClassQuery.MarkingPeriodId);

            return mpcs.ToList();
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

        public override void Setup()
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

           Add(new MarkingPeriodClass
           {
               SchoolRef = 1,
               MarkingPeriodRef = 2,
               ClassRef = 1
           });

           Add(new MarkingPeriodClass
           {
               SchoolRef = 1,
               MarkingPeriodRef = 2,
               ClassRef = 2
           });
        }

        public void Delete(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            foreach (var mpc in markingPeriodClasses)
            {
                Delete(new MarkingPeriodClassQuery
                {
                    MarkingPeriodId = mpc.MarkingPeriodRef,
                    ClassId = mpc.ClassRef
                });
            }
        }
    }
}
