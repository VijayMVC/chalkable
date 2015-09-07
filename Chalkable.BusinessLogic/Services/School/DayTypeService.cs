using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDayTypeService
    {
        void Add(IList<DayType> dayTypes); 
        void Edit(IList<DayType> dayTypes);
        void Delete(IList<DayType> dayTypes);
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
    }
}
