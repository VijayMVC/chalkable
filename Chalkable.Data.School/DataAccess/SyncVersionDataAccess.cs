using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SyncVersionDataAccess : DataAccessBase<SyncVersion, int>
    {
        public SyncVersionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void UpdateVersion(string tableName, int version)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {SyncVersion.TABLE_NAME_FIELD, tableName},
                    {SyncVersion.VERSION_FIELD, version},
                };
            string sql = string.Format("Update SyncVersion Set {0}=@{0} where {1}=@{1}", SyncVersion.VERSION_FIELD, SyncVersion.TABLE_NAME_FIELD);
            ExecuteNonQueryParametrized(sql, ps);
        }
    }
}