using System;
using System.Collections.Generic;
using System.Linq;
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
            return Storage.GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                SchoolYearId = schoolYearId,
                MarkingPeriodId = markingPeriodId
            });
        }

        public GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime date, bool useLastExisting = true)
        {
            var grPeriods = Storage.GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                FromDate = date,
                ToDate = date,
                SchoolYearId = schoolYearId
            });
            return grPeriods.FirstOrDefault();
        }

        public GradingPeriodDetails GetGradingPeriodById(int id)
        {

            return Storage.GradingPeriodStorage
                   .GetGradingPeriodsDetails(new GradingPeriodQuery
                   {
                       GradingPeriodId = id
                   }).First();

           
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            Storage.GradingPeriodStorage.Add(gradingPeriods);
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            Storage.GradingPeriodStorage.Update(gradingPeriods);
        }

        public void Delete(IList<int> ids)
        {
            Storage.GradingPeriodStorage.Delete(ids);
        }

       
    }
}
