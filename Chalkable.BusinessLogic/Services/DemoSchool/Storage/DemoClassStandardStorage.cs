﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStandardStorage:BaseDemoIntStorage<ClassStandard>
    {
        public DemoClassStandardStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public new void Delete(IList<ClassStandard> classStandards)
        {
            foreach (var item in classStandards.Select(classStandard => data.First(
                x =>
                    x.Value.ClassRef == classStandard.ClassRef &&
                    x.Value.StandardRef == classStandard.StandardRef)))
            {
                Delete(item.Key);
            }
        }

        public override void Setup()
        {
            Add(new List<ClassStandard>
            {
                new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    StandardRef = DemoSchoolConstants.MathStandard1
                },

                new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    StandardRef = DemoSchoolConstants.MathStandard2
                },

                 new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.AlgebraClassId,
                    StandardRef = DemoSchoolConstants.MathStandard3
                },

                new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    StandardRef = DemoSchoolConstants.MathStandard1
                },

                new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    StandardRef = DemoSchoolConstants.MathStandard2
                },

                 new ClassStandard
                {
                    ClassRef = DemoSchoolConstants.GeometryClassId,
                    StandardRef = DemoSchoolConstants.MathStandard3
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
