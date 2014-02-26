using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingPeriodService
    {
        IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId = null);
    }

    public class GradingPeriodService : SchoolServiceBase, IGradingPeriodService
    {
        public GradingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId = null)
        {
            using (var uow = Read())
            {
                return new GradingPeriodDataAccess(uow).GetGradingPeriodDetails(schoolYearId, markingPeriodId);
            }
        }
    }
}
