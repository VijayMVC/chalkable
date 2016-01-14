using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPeriodService
    {
        void Delete(IList<int> ids);
        void Edit(IList<Period> periods);
        void AddPeriods(IList<Period> periods); 
        IList<Period> GetPeriods(int schoolYearId);

        IList<Period> GetPeriods(IList<int> ids);
    }

    public class PeriodService : SchoolServiceBase, IPeriodService
    {
        public PeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return DoRead(u => new PeriodDataAccess(u).GetPeriods(schoolYearId));
        }

        public IList<Period> GetPeriods(IList<int> ids)
        {
            return DoRead(u => new PeriodDataAccess(u).GetByIds(ids));
        }

        public void AddPeriods(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new PeriodDataAccess(u).Insert(periods));
        }

        public void Edit(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u=>new PeriodDataAccess(u).Update(periods));
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new PeriodDataAccess(u).Delete(ids));
        }
    }
}
