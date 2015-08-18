using System;
using System.Collections;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Data.School.DataAccess
{
    public class ApplicationInstallActionDataAccess : DataAccessBase<ApplicationInstallAction, int>
    {
        public ApplicationInstallActionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ApplicationInstallAction GetLastAppInstallAction(Guid appId, int ownerId)
        {
            var tname = typeof (ApplicationInstallAction).Name;
            var conds = new AndQueryCondition
            {
                {ApplicationInstallAction.OWNER_REF_FIELD, ownerId},
                {ApplicationInstallAction.APPLICATION_REF_FIELD, appId}
            };
            var q = Orm.OrderedSelect(tname, conds, ApplicationInstallAction.ID_FIELD, Orm.OrderType.Desc, 1);
            return ReadOne<ApplicationInstallAction>(q);
        }

        public IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@applicationid", applicationId}
            };
            return ExecuteStoredProcedureList<ApplicationInstallHistory>("spGetApplicationInstallHistory", ps);
        }
    }
}