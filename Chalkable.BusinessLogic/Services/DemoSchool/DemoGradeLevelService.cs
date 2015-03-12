using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoGradeLevelService : DemoSchoolServiceBase, IGradeLevelService
    {
        public DemoGradeLevelService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return Storage.GradeLevelStorage.GetAll();
        }

        public void Add(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.GradeLevelStorage.Add(gradeLevels);
        }

        public void Delete(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.GradeLevelStorage.Delete(gradeLevels);
        }

        public void Edit(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.GradeLevelStorage.Update(gradeLevels);
        }
    }
}
