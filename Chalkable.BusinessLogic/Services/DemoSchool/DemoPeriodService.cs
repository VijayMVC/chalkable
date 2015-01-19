using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPeriodService : DemoSchoolServiceBase, IPeriodService
    {
        public DemoPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void AddPeriods(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PeriodStorage.Add(periods);
        }

        public void Delete(IList<int> ids)
        {
            Storage.PeriodStorage.Delete(ids);
        }
        
        public void Edit(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PeriodStorage.Update(periods);
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return Storage.PeriodStorage.GetPeriods(schoolYearId);
        }
    }
}
