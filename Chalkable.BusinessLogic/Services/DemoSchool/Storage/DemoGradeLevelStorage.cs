using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradeLevelStorage:BaseDemoStorage<int, GradeLevel>
    {
        public DemoGradeLevelStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<GradeLevel> GetGradeLevels(int? schoolId)
        {
            var schoolGradeLevels = Storage.SchoolGradeLevelStorage.GetAll(schoolId).Select(x => x.GradeLevelRef).ToList();
            return data.Where(x => schoolGradeLevels.Contains(x.Value.Id)).Select(x => x.Value).ToList();
        }

        public void Add(GradeLevel gradeLevel)
        {
            if (!data.ContainsKey(gradeLevel.Id))
                data[gradeLevel.Id] = gradeLevel;
        }

        public void Add(IList<GradeLevel> gradeLevels)
        {
            foreach (var level in gradeLevels)
            {
                Add(level);
            }
        }

        public void Update(IList<GradeLevel> gradeLevels)
        {
            foreach (var gradeLevel in gradeLevels)
            {
                if (data.ContainsKey(gradeLevel.Id))
                    data[gradeLevel.Id] = gradeLevel;
            }
        }

        public void Setup()
        {
            var gradeLevels = new List<GradeLevel>
            {
                new GradeLevel
                {
                    Description = "",
                    Id = 1,
                    Name = "0",
                    Number = 1
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 2,
                    Name = "1",
                    Number = 2
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 3,
                    Name = "2",
                    Number = 3
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 4,
                    Name = "3",
                    Number = 4
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 5,
                    Name = "4",
                    Number = 5
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 7,
                    Name = "5",
                    Number = 6
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 8,
                    Name = "6",
                    Number = 7
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 9,
                    Name = "7",
                    Number = 8
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 10,
                    Name = "8",
                    Number = 9
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 11,
                    Name = "9",
                    Number = 10
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 12,
                    Name = "10",
                    Number = 11
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 13,
                    Name = "11",
                    Number = 12
                },
                new GradeLevel
                {
                    Description = "",
                    Id = 14,
                    Name = "12",
                    Number = 13
                }
            };

            Add(gradeLevels);
        }
    }
}
