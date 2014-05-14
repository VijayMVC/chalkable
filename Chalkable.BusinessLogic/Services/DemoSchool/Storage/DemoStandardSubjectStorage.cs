using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardSubjectStorage:BaseDemoStorage<int, StandardSubject>
    {
        public DemoStandardSubjectStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(IList<StandardSubject> standardSubjects)
        {
            foreach (var standardSubject in standardSubjects)
            {
                if (!data.ContainsKey(standardSubject.Id))
                {
                    data[standardSubject.Id] = standardSubject;
                }
            }
        }

        public void Update(IList<StandardSubject> standardSubjects)
        {
            foreach (var standardSubject in standardSubjects)
            {
                if (data.ContainsKey(standardSubject.Id))
                {
                    data[standardSubject.Id] = standardSubject;
                }
            }
        }

        public override void Setup()
        {
            Add(new List<StandardSubject>
            {
                new StandardSubject()
                {
                    Id = 1,
                    AdoptionYear = 2010,
                    Name = "Math Standards",
                    Description = "",
                    IsActive = true
                },

                new StandardSubject()
                {
                    Id = 2,
                    AdoptionYear = 2010,
                    Name = "Reading Standards",
                    Description = "",
                    IsActive = true
                },

                new StandardSubject()
                {
                    Id = 3,
                    AdoptionYear = 2010,
                    Name = "Science Standards",
                    Description = "",
                    IsActive = true
                },

                new StandardSubject()
                {
                    Id = 4,
                    AdoptionYear = 2100,
                    Name = "Dance Standards",
                    Description = "",
                    IsActive = true
                }
            });
        }
    }
}
