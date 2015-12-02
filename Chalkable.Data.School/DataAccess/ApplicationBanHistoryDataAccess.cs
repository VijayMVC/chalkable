using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ApplicationBanHistoryDataAccess :DataAccessBase<ApplicationBanHistory, int>
    {
        public ApplicationBanHistoryDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ApplicationBanHistory> GetApplicationBanHistory(Guid applicationId)
        {
            var ps = new Dictionary<string, object> {{"applicationId", applicationId}};
            return ExecuteStoredProcedureList<ApplicationBanHistory>("spGetApplicationBanHistory", ps);
        } 
    }
}
