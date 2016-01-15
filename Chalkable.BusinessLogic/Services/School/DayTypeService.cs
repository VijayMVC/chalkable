using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDayTypeService
    {
        void Add(IList<DayType> dayTypes); 
        void Edit(IList<DayType> dayTypes);
        void Delete(IList<DayType> dayTypes);
        IList<DayType> GetDayTypes();
        IList<DayType> GetDayTypes(IList<int> ids);
    }

    public class DayTypeService : SchoolServiceBase, IDayTypeService
    {
        public DayTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<DayType> dayTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<DayType>(u).Insert(dayTypes));
        }

        public void Edit(IList<DayType> dayTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<DayType>(u).Update(dayTypes));
        }

        public void Delete(IList<DayType> dayTypes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<DayType>(u).Delete(dayTypes));
        }

        public IList<DayType> GetDayTypes()
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var conds = new AndQueryCondition {{DayType.SCHOOL_YEAR_REF, Context.SchoolYearId}};
            return DoRead(u => new DataAccessBase<DayType>(u).GetAll(conds));
        }

        public IList<DayType> GetDayTypes(IList<int> ids)
        {
            return DoRead(u => new DataAccessBase<DayType, int>(u).GetByIds(ids));
        }
    }
}
