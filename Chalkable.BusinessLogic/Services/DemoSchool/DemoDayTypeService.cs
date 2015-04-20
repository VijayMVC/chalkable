using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoDayTypeService : DemoSchoolServiceBase, IDayTypeService
    {
        public DemoDayTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }
        
        public void Add(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.DayTypeStorage.Add(dayTypes);
        }

        public void Edit(IList<DayType> dayTypes)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.DayTypeStorage.Update(dayTypes);
        }

        public void Delete(IList<DayType> dayTypes)
        {
            Storage.DayTypeStorage.Delete(dayTypes);
        }
    }
}
