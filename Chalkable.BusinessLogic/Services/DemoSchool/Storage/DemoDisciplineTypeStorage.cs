using System;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDisciplineTypeStorage
    {
        public DisciplineType Add(string name, int score)
        {
            throw new System.NotImplementedException();
        }

        public DisciplineType GetDisciplineTypeById(Guid id)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count)
        {
            throw new NotImplementedException();
        }

        public DisciplineType Edit(Guid id, string name, int score)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
