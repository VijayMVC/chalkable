using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardSubjectStorage:BaseDemoIntStorage<StandardSubject>
    {
        public DemoStandardSubjectStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
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
                    Id = DemoSchoolConstants.ScienceStandardSubject,
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
