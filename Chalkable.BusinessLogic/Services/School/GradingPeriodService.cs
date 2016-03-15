using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingPeriodService
    {
        IList<GradingPeriod> GetGradingPeriodsDetailsByClassId(int classId);
        IList<GradingPeriod> GetGradingPeriodsDetails(int schoolYearId);
        GradingPeriod GetGradingPeriodDetails(int schoolYearId, DateTime date);
        GradingPeriod GetGradingPeriodById(int id);
        void Add(IList<GradingPeriod> gradingPeriods);
        void Edit(IList<GradingPeriod> gradingPeriods);
        void Delete(IList<int> ids);
    }

    public class GradingPeriodService : SchoolServiceBase, IGradingPeriodService
    {
        public GradingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradingPeriod> GetGradingPeriodsDetails(int schoolYearId)
        {
            using (var uow = Read())
            {
                return new GradingPeriodDataAccess(uow).GetGradingPeriodsDetails(new GradingPeriodQuery
                    {
                        SchoolYearId = schoolYearId
                    });
            }
        }

        public IList<GradingPeriod> GetGradingPeriodsDetailsByClassId(int classId)
        {
            using (var uow = Read())
            {
                return new GradingPeriodDataAccess(uow).GetGradingPeriodsDetails(new GradingPeriodQuery
                {
                    ClassId = classId
                });
            }
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Insert(gradingPeriods);
                uow.Commit();
            }
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Update(gradingPeriods);
                uow.Commit();
            }
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new GradingPeriodDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }

        public GradingPeriod GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            using (var uow = Read())
            {
                var da = new GradingPeriodDataAccess(uow);
                var gradingPeriodQuery = new GradingPeriodQuery
                    {
                        SchoolYearId = schoolYearId
                    };
                var gps = da.GetGradingPeriodsDetails(gradingPeriodQuery);
                var res = gps.FirstOrDefault(x => x.StartDate <= date && x.EndDate >= date);
                if (res == null)
                    res = gps.OrderByDescending(x => x.StartDate).FirstOrDefault();
                return res;
            }
        }

        public GradingPeriod GetGradingPeriodById(int id)
        {
            using (var uow = Read())
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
