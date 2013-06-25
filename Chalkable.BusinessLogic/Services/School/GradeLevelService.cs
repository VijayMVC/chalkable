using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels();
    }
    public class GradeLevelService : SchoolServiceBase, IGradeLevelService
    {
        public GradeLevelService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            using (var uow = Read())
            {
                var da = new GradeLevelDataAccess(uow);
                return da.GetAll();
            }
        }
    }
}
