using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoGradeLevelStorage : BaseDemoIntStorage<GradeLevel>
    {
        public DemoGradeLevelStorage()
            : base(x => x.Id)
        {
        }

        public IList<GradeLevel> GetGradeLevels(int? schoolId)
        {
            var schoolGradeLevels = StorageLocator.SchoolGradeLevelStorage.GetAll(schoolId).Select(x => x.GradeLevelRef).ToList();
            return data.Where(x => schoolGradeLevels.Contains(x.Value.Id)).Select(x => x.Value).ToList();
        }
    }
  
    public class DemoGradeLevelService : DemoSchoolServiceBase, IGradeLevelService
    {
        private DemoGradeLevelStorage GradeLevelStorage { get; set; }
        public DemoGradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradeLevelStorage = new DemoGradeLevelStorage();
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return GradeLevelStorage.GetAll();
        }

        public void Add(IList<GradeLevel> gradeLevels)
        {
            GradeLevelStorage.Add(gradeLevels);
        }

        public void Delete(IList<GradeLevel> gradeLevels)
        {
            GradeLevelStorage.Delete(gradeLevels);
        }

        public void Edit(IList<GradeLevel> gradeLevels)
        {
            GradeLevelStorage.Update(gradeLevels);
        }
    }
}
