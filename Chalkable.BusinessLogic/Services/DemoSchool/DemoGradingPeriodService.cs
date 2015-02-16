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

        public IList<GradingPeriod> GetGradingPeriodsDetails(int schoolYearId, int? classId = null)
        {
            return Storage.GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                SchoolYearId = schoolYearId,
                ClassId = classId
            });
        }

        public GradingPeriod GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            var gps = Storage.GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                SchoolYearId = schoolYearId
            }); 
            var res = gps.FirstOrDefault(x => x.StartDate <= date && x.EndDate >= date);
            if (res == null)
                res = gps.OrderByDescending(x => x.StartDate).FirstOrDefault();
            return res;
        }

        public GradingPeriod GetGradingPeriodById(int id)
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
