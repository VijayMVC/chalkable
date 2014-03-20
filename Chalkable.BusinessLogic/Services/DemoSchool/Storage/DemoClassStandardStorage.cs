using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStandardStorage:BaseDemoStorage<int, ClassStandard>
    {
        public DemoClassStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<ClassStandard> Add(IList<ClassStandard> classStandarts)
        {
            throw new System.NotImplementedException();
        }
    }
}
