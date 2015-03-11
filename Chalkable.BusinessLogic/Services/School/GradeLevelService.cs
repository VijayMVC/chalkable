using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradeLevelService
    {
        IList<GradeLevel> GetGradeLevels();
        void Add(IList<GradeLevel> gradeLevels);
        void Delete(IList<int> ids);
        void Edit(IList<GradeLevel> gradeLevels);
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
        
        public void Add(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(uow => new GradeLevelDataAccess(uow).Insert(gradeLevels));
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }

        public void Edit(IList<GradeLevel> gradeLevels)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new GradeLevelDataAccess(uow).Update(gradeLevels);
                uow.Commit();
            }
        }
    }
}
