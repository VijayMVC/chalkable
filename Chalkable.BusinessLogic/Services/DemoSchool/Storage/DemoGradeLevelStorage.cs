using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }

        public void Add(GradeLevel gradeLevel)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<GradeLevel> gradeLevel)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<GradeLevel> gradeLevels)
        {
            throw new System.NotImplementedException();
        }
    }
}
