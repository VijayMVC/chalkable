using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DistrictDataAccess : DataAccessBase<District, Guid>
    {
        public DistrictDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public IDictionary<string, int> CalcServersLoading()
        {
            var res = Settings.Servers.ToDictionary(server => server, server => 0);
            string sql = string.Format("select {0}, count(*) as [Count] from District group by {0}", District.SERVER_URL_FIELD);
            using (var reader = ExecuteReaderParametrized(sql, new Dictionary<string, object>()))
            {
                while (reader.Read())
                {
                    var server = SqlTools.ReadStringNull(reader, District.SERVER_URL_FIELD);
                    var count = SqlTools.ReadInt32(reader, "Count");
                    res[server] = count;
                }
            }
            return res;
        }
        
        public new void Delete(Guid id)
        {
            using (ExecuteStoredProcedureReader("spDeleteDistrict ", new Dictionary<string, object> { { "id", id } })) { }
        }

        public PaginatedList<DistrictSyncStatus> GetSyncStatuses(int start, int count)
        {
            var sql = "select * from vwDistrictSyncStatus";
            return PaginatedSelect<DistrictSyncStatus>(new DbQuery(sql, null), District.ID_FIELD, start, count);
        }

        //---------------------------------------------------------------------
        //TODO: next methods runs on the district db server under master database
        //---------------------------------------------------------------------

        public void CreateDistrictDataBase(string dbName, string prototypeName)
        {
            //TODO: what if servers are different?
            var sql = string.Format("Create Database [{0}] as copy of [{1}]", dbName, prototypeName);
            ExecuteNonQueryParametrized(sql, new Dictionary<string, object>());
        }
        
        public IList<string> GetOnline(IEnumerable<Guid> names)
        {
            var sql = string.Format("SELECT name FROM sys.databases WHERE name  in ({0}) and state = 0",
                                    names.Select(x => "'" + x + "'").JoinString(","));
            var res = new List<string>();
            using (var reader = ExecuteReaderParametrized(sql, new Dictionary<string, object>()))
                while (reader.Read())
                    res.Add(SqlTools.ReadStringNull(reader, "name"));
            return res;
        }

        public void DeleteDistrictDataBase(string name)
        {
            var sql = string.Format("drop database [{0}]", name);
            ExecuteNonQueryParametrized(sql, new Dictionary<string, object>());
        }
    }
}