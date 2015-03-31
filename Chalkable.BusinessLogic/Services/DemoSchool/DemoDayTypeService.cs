using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoDayTypeStorage : BaseDemoIntStorage<DayType>
    {
        public DemoDayTypeStorage()
            : base(x => x.Id)
        {
        }

        public IList<DayType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var dayTypes = data.Select(x => x.Value).Where(x => x.SchoolYearRef == schoolYearId);

            if (fromNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number >= fromNumber);
            if (toNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number <= toNumber);

            return dayTypes.ToList();
        }

        public bool Exists(int schoolYearId)
        {
            return data.Count(x => x.Value.SchoolYearRef == schoolYearId) > 0;
        }
    }

    public class DemoDayTypeService : DemoSchoolServiceBase, IDayTypeService
    {
        private DemoDayTypeStorage DayTypeStorage { get; set; }
        public DemoDayTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            DayTypeStorage = new DemoDayTypeStorage();
        }
        
        public void Add(IList<DayType> dayTypes)
        {
            DayTypeStorage.Add(dayTypes);
        }

        public void Edit(IList<DayType> dayTypes)
        {
            DayTypeStorage.Update(dayTypes);
        }

        public void Delete(IList<DayType> dayTypes)
        {
            DayTypeStorage.Delete(dayTypes);
        }

        public bool Exists(int schoolYearId)
        {
            return DayTypeStorage.Exists(schoolYearId);
        }
    }
}
