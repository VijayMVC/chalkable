using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Chalkable.Common;

namespace Chalkable.Data.Common
{
    public static class Orm
    {
        private static bool IsDbField(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DataEntityAttr>() == null
                && propertyInfo.GetCustomAttribute<NotDbFieldAttr>() == null;
        }

        public static List<string> Fields(Type t)
        {
            var res = new List<string>();
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    if (IsDbField(propertyInfo))
                        res.Add(propertyInfo.Name);
                }
            return res;
        }

        public static List<string> Fields<T>()
        {
            var t = typeof (T);
            return Fields(t);
        }

        private const string COMPLEX_RESULT_SET_FORMAT = " {0}.{1} as {0}_{1}";
        public static IList<string> ComplexFields(Type t)
        {
            var fields = Fields(t);
            var objName = t.Name;
            return fields.Select(x => string.Format(COMPLEX_RESULT_SET_FORMAT, objName, x)).ToList();
        } 

        public static string ComplexResultSetQuery(IList<Type> types)
        {
            var res = new StringBuilder();
            foreach (var type in types)
            {
                res.Append(ComplexFields(type).JoinString(","));
            }
            return res.ToString();
        }

        public static DbQuery SimpleInsert<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof (T);
            var fields = Fields(t);
            b.Append("Insert into [").Append(t.Name).Append("] (");
            b.Append(fields.Select(x=>"[" + x + "]").JoinString(",")).Append(") values (");
            b.Append(fields.Select(x => "@" + x).JoinString(",")).Append(")");
            res.Sql = b.ToString();
            foreach (var field in fields)
            {
                res.Parameters.Add("@" + field, t.GetProperty(field).GetValue(obj));
            }
            return res;
        }

        public static DbQuery SimpleUpdate<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof(T);
            var fields = Fields(t);
            
            b.Append("Update [").Append(t.Name).Append("] set ");
            b.Append(fields.Select(x => "[" + x + "]=@" + x).JoinString(",")).Append(" where [Id] = @Id");
            res.Sql = b.ToString();
            foreach (var field in fields)
            {
                res.Parameters.Add("@" + field, t.GetProperty(field).GetValue(obj));
            }
            return res;
        }

        public static DbQuery SimpleDelete<T>(T obj)
        {
            var res = new DbQuery();
            res.Parameters = new Dictionary<string, object>();
            var b = new StringBuilder();
            var t = typeof(T);
            b.Append("Delete from [").Append(t.Name).Append("] where Id = @Id");
            res.Parameters.Add("@Id", t.GetProperty("Id").GetValue(obj));
            res.Sql = b.ToString();
            return res;
        }

        public static DbQuery SimpleDelete<T>(Dictionary<string, object> conditioins)
        {
            var res = new DbQuery { Parameters = conditioins };
            var b = new StringBuilder();
            var t = typeof(T);
            b.AppendFormat("Delete from [{0}]", t.Name);
            b = BuildSqlWhere(b, t, conditioins);
            res.Sql = b.ToString();
            return res;
        }

        public static StringBuilder BuildSqlWhere(StringBuilder builder, Type t, Dictionary<string, object> conds)
        {
            if (conds != null && conds.Count > 0)
            {
                builder.Append(" where ");
                bool first = true;
                foreach (var cond in conds)
                {
                    if (first) first = false;
                    else
                    {
                        builder.Append(" and ");
                    }
                    builder.AppendFormat("[{0}].{1} =@{1}", t.Name, cond.Key);
                }
            }
            return builder;
        }

        
        public static DbQuery SimpleSelect<T>(Dictionary<string, object> conds)
        {
            var res = new DbQuery {Parameters = conds};
            var b = new StringBuilder();
            var t = typeof (T);
            b.AppendFormat("Select * from [{0}]", t.Name);
            b = BuildSqlWhere(b, t, conds);
            res.Sql = b.ToString();
            return res;
        }

        public static DbQuery CountSelect<T>(Dictionary<string, object> conds, string resultName)
        {
            var b = new StringBuilder();
            var res = SimpleSelect<T>(conds);
            b.AppendFormat("Select Count(*) as {1} from ({0})", res.Sql, resultName);
            res.Sql = b.ToString();
            return res;
        }

        public static DbQuery PaginationSelect<T>(Dictionary<string, object> conds, string orderColumn, int start, int count)
        {
            var b = new StringBuilder();
            var res = SimpleSelect<T>(conds);
            b.Append(res.Sql);
            b.AppendFormat("select count(*) as AllCount from ({0});", res.Sql);
            b.Append("select y.* from (");
            b.AppendFormat("  select x.*, row_number() over(order by x.{1}) as RowNumber from ({0}) x", res.Sql, orderColumn);
            b.AppendFormat(")y where RowNumber >= {0} and RowNumber <= {1}", start, count);
            b.AppendFormat("order by x.{0}", orderColumn);
            res.Sql = b.ToString();
            return res;
        }
    }

    public class DbQuery
    {
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 
    }
}
