using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardStorage:BaseDemoStorage<int ,Standard>
    {
        public DemoStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public void AddStandards(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                standard.Id = GetNextFreeId();
                data[standard.Id] = standard;
            }
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

        public void Update(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }

        public override void Setup()
        {
            AddStandards(new List<Standard>
            {
                new Standard
                {
                    IsActive = true,
                    Name = "Math 5",
                    LowerGradeLevelRef = 7,
                    UpperGradeLevelRef = 7,
                    StandardSubjectRef = 1
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Reading -1",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Reading -2",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Reading -3",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Reading -4",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Grammar -1",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Grammar -2",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 2
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Science -1",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 3
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Science -2",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 3
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Science -1.1",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 3,
                    ParentStandardRef = 8
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Science -1.2",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 3,
                    ParentStandardRef = 8
                },

                new Standard
                {
                    IsActive = true,
                    Name = "Dancing -1",
                    LowerGradeLevelRef = 1,
                    UpperGradeLevelRef = 14,
                    StandardSubjectRef = 4
                }

            });  
        }
    }
}
