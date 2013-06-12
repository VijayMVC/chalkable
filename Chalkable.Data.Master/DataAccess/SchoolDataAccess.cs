using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolDataAccess : DataAccessBase
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
            return SelectMany<School>(new Dictionary<string, object> {{School.IS_EMPTY_FIELD, true}});
        }

        public School GetById(Guid id)
        {
            return SelectOne<School>(new Dictionary<string, object> { { "Id", id } });
        }

        //TODO: next methods runs on the school db server under master database
        public void CreateSchoolDataBase(string dbName)
        {
            //TODO: what if servers are different?
            var sql = string.Format("Create Database [{0}] as copy of [{1}]", dbName, Settings.Configuration.SchoolTemplateDataBase);
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

        
    }
}