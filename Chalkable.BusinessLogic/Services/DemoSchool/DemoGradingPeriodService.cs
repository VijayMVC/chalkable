using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingPeriodService : DemoSchoolServiceBase, IGradingPeriodService
    {
        public DemoGradingPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId = null)
        {
            return Storage.GradingPeriodStorage.GetGradingPeriodDetails(schoolYearId, markingPeriodId);
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            throw new System.NotImplementedException();
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IList<int> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}
