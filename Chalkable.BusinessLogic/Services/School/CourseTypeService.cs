using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICourseTypeService
    {
        void Add(IList<CourseType> courseTypes);
        void Edit(IList<CourseType> courseTypes);
        void Delete(IList<CourseType> courseTypes);
        IList<CourseType> GetList(bool activeOnly, string filter = null);
    }

    public class CourseTypeService : SchoolServiceBase, ICourseTypeService
    {
        public CourseTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<CourseType> courseTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=> new DataAccessBase<CourseType>(u).Insert(courseTypes));
        }

        public void Edit(IList<CourseType> courseTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<CourseType>(u).Update(courseTypes));

        }

        public void Delete(IList<CourseType> courseTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<CourseType>(u).Delete(courseTypes));
        }

        public IList<CourseType> GetList(bool activeOnly, string filter = null)
        {
            var conds = activeOnly ? new AndQueryCondition {{CourseType.IS_ACTIVE_FIELD, true}} : null;
            return DoRead(u => new ClassDataAccess(u).CourseTypes(conds, filter));
        }
    }
}
