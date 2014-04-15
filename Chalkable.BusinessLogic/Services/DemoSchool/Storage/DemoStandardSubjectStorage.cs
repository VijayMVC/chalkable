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
            throw new System.NotImplementedException();
        }
    }
}
