﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStandardStorage:BaseDemoIntStorage<ClassStandard>
    {
        public DemoClassStandardStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public void Delete(IList<ClassStandard> classStandards)
        {
            foreach (var classStandard in classStandards)
            {
                var item =
                    data.First(
                        x =>
                            x.Value.ClassRef == classStandard.ClassRef &&
                            x.Value.StandardRef == classStandard.StandardRef);
                Delete(item.Key);
            }
        }

        public override void Setup()
        {
            Add(new List<ClassStandard>
            {
                new ClassStandard
                {
                    ClassRef = 1,
                    StandardRef = 1
                },

                new ClassStandard
                {
                    ClassRef = 1,
                    StandardRef = 8
                },

                 new ClassStandard
                {
                    ClassRef = 1,
                    StandardRef = 9
                },

                new ClassStandard
                {
                    ClassRef = 2,
                    StandardRef = 1
                },

                new ClassStandard
                {
                    ClassRef = 2,
                    StandardRef = 8
                },

                 new ClassStandard
                {
                    ClassRef = 2,
                    StandardRef = 9
                }
            });
        }

        public IList<ClassStandard> GetAll(int? classId)
        {
            var items = data.Select(x => x.Value);
            if (classId.HasValue)
                items = items.Where(x => x.ClassRef == classId);
            return items.ToList();
        }
    }
}
