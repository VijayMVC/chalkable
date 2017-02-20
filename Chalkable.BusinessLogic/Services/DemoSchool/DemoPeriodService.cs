using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPeriodStorage : BaseDemoIntStorage<Period>
    {
        public DemoPeriodStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return data.Where(x => x.Value.SchoolYearRef == schoolYearId).Select(x => x.Value).ToList();
        }

    }

    public class DemoPeriodService : DemoSchoolServiceBase, IPeriodService
    {
        private DemoPeriodStorage PeriodStorage { get; set; }
        public DemoPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PeriodStorage = new DemoPeriodStorage();
        }

        public void AddPeriods(IList<Period> periods)
        {
            PeriodStorage.Add(periods);
        }

        public void Delete(IList<int> ids)
        {
            PeriodStorage.Delete(ids);
        }
        
        public void Edit(IList<Period> periods)
        {
            PeriodStorage.Update(periods);
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return PeriodStorage.GetPeriods(schoolYearId);
        }

        public IList<Period> GetPeriods(IList<int> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
