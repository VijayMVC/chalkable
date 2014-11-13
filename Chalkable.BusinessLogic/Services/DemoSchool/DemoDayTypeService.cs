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


        public IList<DayType> GetSections(int schoolYearId)
        {
            return Storage.DayTypeStorage.GetDateTypes(schoolYearId);
            
        }

        //TODO : filter by school 
        public DayType GetSectionById(int id)
        {
            return Storage.DayTypeStorage.GetById(id);
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

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                var dateType = Storage.DayTypeStorage.GetById(id);
                Storage.DayTypeStorage.Delete(dateType);
            }
        }
    }
}
