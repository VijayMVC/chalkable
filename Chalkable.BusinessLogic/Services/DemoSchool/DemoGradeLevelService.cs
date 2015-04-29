using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolGradeLevelStorage : BaseDemoIntStorage<SchoolGradeLevel>
    {
        public DemoSchoolGradeLevelStorage()
            : base(null, true)
        {
        }

        public IList<SchoolGradeLevel> GetAll(int? schoolId)
        {
            return data.Where(x => x.Value.SchoolRef == schoolId).Select(x => x.Value).ToList();
        }
    }

    public class DemoGradeLevelStorage : BaseDemoIntStorage<GradeLevel>
    {
        public DemoGradeLevelStorage()
            : base(x => x.Id)
        {
        }
    }
  
    public class DemoGradeLevelService : DemoSchoolServiceBase, IGradeLevelService
    {
        private DemoGradeLevelStorage GradeLevelStorage { get; set; }
        private DemoSchoolGradeLevelStorage SchoolGradeLevelStorage { get; set; }
        public DemoGradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradeLevelStorage = new DemoGradeLevelStorage();
            SchoolGradeLevelStorage = new DemoSchoolGradeLevelStorage();
        }

        public IList<GradeLevel> GetGradeLevels(int? schoolId)
        {
            var schoolGradeLevels = SchoolGradeLevelStorage.GetAll(schoolId).Select(x => x.GradeLevelRef).ToList();
            return GradeLevelStorage.GetData().Where(x => schoolGradeLevels.Contains(x.Value.Id)).Select(x => x.Value).ToList();
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return GradeLevelStorage.GetAll();
        }

        public GradeLevel GetGradeLevelById(int gradeLevelId)
        {
            return GradeLevelStorage.GetById(gradeLevelId);
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
