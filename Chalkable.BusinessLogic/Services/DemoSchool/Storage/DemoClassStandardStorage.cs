using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStandardStorage:BaseDemoStorage<int, ClassStandard>
    {
        private int index = 0;
        public DemoClassStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<ClassStandard> Add(IList<ClassStandard> classStandards)
        {
            foreach (var classStandard in classStandards)
            {
                data.Add(index++, classStandard);
            }
            return classStandards;
        }
    }
}
