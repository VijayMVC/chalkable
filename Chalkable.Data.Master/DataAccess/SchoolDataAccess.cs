using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<School>
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(School school)
        {
            SimpleInsert(school);
        }

        public IDictionary<string, int> CalcServersLoading()
        {
            var res = Settings.Servers.ToDictionary(server => server, server => 0);
            string sql = string.Format("select {0}, count(*) as [Count] from School group by {0}", School.SERVER_URL_FIELD);
            using (var reader = ExecuteReaderParametrized(sql, new Dictionary<string, object>()))
            {
                while (reader.Read())
                {
                    var server = SqlTools.ReadStringNull(reader, School.SERVER_URL_FIELD);
                    var count = SqlTools.ReadInt32(reader, "Count");
                    res[server] = count;
                }
            }
            return res;
        }

        public IList<School> GetEmpty()
        {
            return SelectMany<School>(new AndQueryCondition {{School.IS_EMPTY_FIELD, true}});
        }

        public IList<School> GetSchools()
        {
            return SelectMany<School>(new AndQueryCondition());
        } 

        //TODO: next methods runs on the school db server under master database
        public void CreateSchoolDataBase(string dbName, string prototypeName)
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
        
        public PaginatedList<School> GetSchools(Guid? districtId, int start, int count)
        {
            var conds = new AndQueryCondition();
            if(districtId.HasValue)
                conds.Add(School.DISTRICT_REF_FIELD, districtId);
            return PaginatedSelect<School>(conds, School.ID_FIELD, start, count);
        }
 

        public IList<School> GetSchools(bool? empty, bool? demo, bool? usedDemo)
        {
            var conds = new AndQueryCondition();
            if (empty.HasValue)
                conds.Add(School.IS_EMPTY_FIELD, empty);
            if (demo.HasValue)
                conds.Add(School.DEMO_PREFIX_FIELD, null, demo.Value ? ConditionRelation.NotEqual : ConditionRelation.Equal);
            if (usedDemo.HasValue)
                conds.Add(School.LAST_USED_DEMO_FIELD, null, usedDemo.Value ? ConditionRelation.NotEqual : ConditionRelation.Equal);
            return SelectMany<School>(conds);
        }

        public SisSync GetSyncData(Guid schoolId)
        {
            return SelectOneOrNull<SisSync>(new AndQueryCondition { { "Id", schoolId } });
        }

        public void SetSyncData(SisSync sisSync)
        {
            if (GetSyncData(sisSync.Id) != null)
                SimpleUpdate(sisSync);
            else
                SimpleInsert(sisSync);
        }
    }
}