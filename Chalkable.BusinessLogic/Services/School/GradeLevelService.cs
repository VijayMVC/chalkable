using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels();
        void Add(IList<GradeLevel> gradeLevels);
        void Delete(IList<GradeLevel> gradeLevels);
        void Edit(IList<GradeLevel> gradeLevels);
    }
    public class GradeLevelService : SchoolServiceBase, IGradeLevelService
    {
        public GradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return DoRead(u => new DataAccessBase<GradeLevel>(u).GetAll().OrderBy(x=>x.Number).ToList());
        }
        
        public void Add(IList<GradeLevel> gradeLevels)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradeLevel>(u).Insert(gradeLevels));
        }

        public void Delete(IList<GradeLevel> gradeLevels)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradeLevel>(u).Delete(gradeLevels));
        }

        public void Edit(IList<GradeLevel> gradeLevels)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<GradeLevel>(u).Update(gradeLevels));
        }
    }
}
