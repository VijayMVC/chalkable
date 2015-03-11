using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

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
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u=>new DayTypeDataAccess(u).Insert(dayTypes));
        }

        public void Edit(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DayTypeDataAccess(u).Update(dayTypes));
        }

        public void Delete(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DayTypeDataAccess(u).Delete(dayTypes));
        }
    }
}
