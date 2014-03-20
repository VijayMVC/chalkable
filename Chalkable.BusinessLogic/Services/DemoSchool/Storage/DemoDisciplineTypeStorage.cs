using System;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDisciplineTypeStorage:BaseDemoStorage<Guid, DisciplineType>
    {
        public DemoDisciplineTypeStorage(DemoStorage storage) : base(storage)
        {
        }

        public DisciplineType Add(string name, int score)
        {
            throw new System.NotImplementedException();
        }


        public PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count)
        {
            throw new NotImplementedException();
        }

        public DisciplineType Edit(Guid id, string name, int score)
        {
            throw new NotImplementedException();
        }

    }
}
