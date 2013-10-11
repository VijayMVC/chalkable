using System.Collections.Generic;
using System.Text;

namespace Chalkable.Data.Common.Orm
{
    public class DbQuery
    {
        public StringBuilder Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 

        public DbQuery ()
        {
            Sql = new StringBuilder();
            Parameters = new Dictionary<string, object>();
        }

        public DbQuery(string sql, IDictionary<string, object> parameters)
        {
            Sql = new StringBuilder(sql);
            Parameters = parameters;
        }

        public DbQuery(StringBuilder sql, IDictionary<string, object> parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }
    }
}