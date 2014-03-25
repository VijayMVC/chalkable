using System;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDisciplineTypeService : DemoSchoolServiceBase, IDisciplineTypeService
    {
        public DemoDisciplineTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public DisciplineType Add(string name, int score)
        {
            return Storage.DisciplineTypeStorage.Add(name, score);
        }

        public DisciplineType Edit(Guid id, string name, int score)
        {
            return Storage.DisciplineTypeStorage.Edit(id, name, score);
        }

        public void Delete(Guid id)
        {
            Storage.DisciplineTypeStorage.Delete(id);
        }

        public PaginatedList<DisciplineType> GetDisciplineTypes(int start, int count)
        {
            return Storage.DisciplineTypeStorage.GetDisciplineTypes(start, count);
        }

        public DisciplineType GetDisciplineTypeById(Guid id)
        {
            return Storage.DisciplineTypeStorage.GetById(id);

        }
    }
}
