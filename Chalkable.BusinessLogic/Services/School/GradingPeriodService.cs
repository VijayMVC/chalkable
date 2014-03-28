using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingPeriodService
    {
        IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId = null);
        GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime tillDate);
        GradingPeriodDetails GetGradingPeriodById(int id);
        void Add(IList<GradingPeriod> gradingPeriods);
        void Edit(IList<GradingPeriod> gradingPeriods);
        void Delete(IList<int> ids);
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
                return new GradingPeriodDataAccess(uow).GetGradingPeriodsDetails(new GradingPeriodQuery
                    {
                        SchoolYearId = schoolYearId,
                        MarkingPeriodId = markingPeriodId
                    });
            }
        }

        private void ValidateGradingPeriods(IEnumerable<GradingPeriod> gradingPeriods)
        {
            if (gradingPeriods.Any(gradingPeriod => gradingPeriod.StartDate > gradingPeriod.EndDate))
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);     
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ValidateGradingPeriods(gradingPeriods);
            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Insert(gradingPeriods);
                uow.Commit();
            }
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            
            ValidateGradingPeriods(gradingPeriods);
            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Update(gradingPeriods);
                uow.Commit();
            }
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }
        
        public GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            using (var uow = Update())
            {
                var da = new GradingPeriodDataAccess(uow);
                var grPeriods = da.GetGradingPeriodsDetails(new GradingPeriodQuery
                    {
                        FromDate = date,
                        ToDate = date,
                        SchoolYearId = schoolYearId
                    });
                return grPeriods.FirstOrDefault();
            }
        }

        public GradingPeriodDetails GetGradingPeriodById(int id)
        {
            using (var uow = Update())
            {
                return new GradingPeriodDataAccess(uow)
                    .GetGradingPeriodsDetails(new GradingPeriodQuery
                    {
                        GradingPeriodId = id
                    }).First();
            }
        }
    }
}
