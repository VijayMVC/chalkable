using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardStorage:BaseDemoIntStorage<Standard>
    {
        public DemoStandardStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public void AddStandards(IList<Standard> standards)
        {
            Add(standards);
        }

        public IList<Standard> GetStandarts(StandardQuery query)
        {
            var standards = data.Select(x => x.Value);
            if (query.StandardSubjectId.HasValue)
                standards = standards.Where(x => x.StandardSubjectRef == query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
                standards =
                    standards.Where(
                        x => query.GradeLavelId <= x.LowerGradeLevelRef && query.GradeLavelId >= x.UpperGradeLevelRef);
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                standards = standards.Where(x => x.ParentStandardRef == query.ParentStandardId);

            if (query.ClassId.HasValue)
            {
                var classStandarts = Storage.ClasStandardStorage.GetAll(query.ClassId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }
            if (query.CourseId.HasValue)
            {
                var classStandarts = Storage.ClasStandardStorage.GetAll(query.CourseId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }

            return standards.ToList();
        }

        public override void Setup()
        {
            AddStandards(new List<Standard>
            {
                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard1,
                    IsActive = true,
                    Name = "Math 1",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel10,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
                },

                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard2,
                    IsActive = true,
                    Name = "Math 2",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel11,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel11,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
                },

                new Standard
                {
                    Id = DemoSchoolConstants.MathStandard3,
                    IsActive = true,
                    Name = "Math 3",
                    LowerGradeLevelRef = DemoSchoolConstants.GradeLevel12,
                    UpperGradeLevelRef = DemoSchoolConstants.GradeLevel12,
                    StandardSubjectRef = DemoSchoolConstants.ScienceStandardSubject
                },

            });  
        }
    }
}
