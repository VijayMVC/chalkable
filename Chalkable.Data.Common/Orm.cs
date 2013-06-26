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

        public enum OrderType
        {
            Asc = 1,
            Desc = 2
        }
        private static IDictionary<OrderType, string> orederTypesMap = new Dictionary<OrderType, string>
            {
                {OrderType.Asc, "ASC"},
                {OrderType.Desc, "DESC"}
            }; 

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
        public static IList<string> FullFieldsNames(Type t)
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
                res.Append(FullFieldsNames(type).JoinString(","));
            }
            return res.ToString();
        }

        //TODO: insert from select 

        public static DbQuery SimpleInsert<T>(T obj)
        {
            return SimpleListInsert(new List<T> { obj });
        } 

        public static DbQuery SimpleListInsert<T>(IList<T> objs)
        {
            var res = new DbQuery {Parameters = new Dictionary<string, object>()};
            var b = new StringBuilder();
            var t = typeof(T);
            var fields = Fields(t);
            b.Append("Insert into [").Append(t.Name).Append("] (");
            b.Append(fields.Select(x => "[" + x + "]").JoinString(",")).Append(")");
            b.Append(" values ");

            for (int i = 0; i < objs.Count; i++)
            {
                b.Append("(").Append(fields.Select(x => "@" + x + "_" + i.ToString()).JoinString(",")).Append(")");
                if (i != objs.Count - 1)
                {
                    b.Append(",");
                }
                foreach (var field in fields)
                {
                    var fieldValue = t.GetProperty(field).GetValue(objs[i]);
                    res.Parameters.Add(field + "_" + i.ToString(), fieldValue);
                }                
            }
            res.Sql = b.ToString();
            return res;
        }


         public static DbQuery SimpleUpdate<T>(T obj)
         {
             var t = typeof(T);
             var fields = Fields(t);
             var updateParams = fields.ToDictionary(field => field, field => t.GetProperty(field).GetValue(obj));
             var conds = new Dictionary<string, object> {{"id", t.GetProperty("Id").GetValue(obj)}};
             return SimpleUpdate(t, fields.Select(x => x.ToLower()).ToList(), updateParams, conds);
         }

         public static DbQuery SimpleUpdate<T>(Dictionary<string, object> updateParams, Dictionary<string, object> conditions)
         {
             var t = typeof(T);
             var fields = Fields(t).Select(x => x.ToLower()).ToList();
             return SimpleUpdate(t, fields, updateParams, conditions);
         }

         private static DbQuery SimpleUpdate(Type t, IList<string> fields, Dictionary<string, object> updateParams,
                                             Dictionary<string, object> conditions)
         {
            var res = new DbQuery {Parameters = new Dictionary<string, object>()};
            var b = new StringBuilder();
            b.Append("Update [").Append(t.Name).Append("] set");
            var setParams = new Dictionary<string, string>();
            var setParamsPrefix = "set_param_";
            foreach (var updateParam in updateParams)
            {
                if (!fields.Contains(updateParam.Key))
                {
                    var setParamName = setParamsPrefix + updateParam.Key;
                    if (!res.Parameters.ContainsKey(setParamName))
                        res.Parameters.Add(setParamName, updateParam.Value);
                    if (!setParams.ContainsKey(updateParam.Key))
                        setParams.Add(updateParam.Key, setParamName);       
                }
            }
            b.Append(setParams.Select(x=> "["  + x.Key + "]=@" + x.Value).JoinString(","));
            b = BuildSqlWhere(b, t, conditions);
            foreach (var condition in conditions)
            {
                if(!res.Parameters.ContainsKey(condition.Key))
                    res.Parameters.Add(condition);
            }
            res.Sql = b.ToString();
            return res;
         }


        public static DbQuery SimpleDelete<T>(T obj)
        {
            var t = typeof(T);
            var conds = new Dictionary<string, object> {{"Id", t.GetProperty("Id").GetValue(obj)}};
            return SimpleDelete<T>(conds);
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
                builder.Append(" where (");
                bool first = true;
                foreach (var cond in conds)
                {
                    if (first) first = false;
                    else
                    {
                        builder.Append(" and ");
                    }
                    if (cond.Value != null)
                        builder.AppendFormat("[{0}].{1} =@{1}", t.Name, cond.Key);
                    else
                        builder.AppendFormat("[{0}].{1} is null", t.Name, cond.Key);
                }
                builder.Append(")");
            }
            return builder;
        }

        
        public static DbQuery SimpleSelect<T>(Dictionary<string, object> conds)
        {
            var res = new DbQuery {Parameters = conds};
            var b = new StringBuilder();
            var t = typeof (T);
            b.AppendFormat("Select * from [{0}] ", t.Name);
            b = BuildSqlWhere(b, t, conds);
            res.Sql = b.ToString();
            return res;
        }

        public static DbQuery CountSelect<T>(Dictionary<string, object> conds, string resultName)
        {
            return CountSelect(SimpleSelect<T>(conds), resultName);
        }

        public static DbQuery CountSelect(DbQuery query, string resultName)
        {
            var b = new StringBuilder();
            b.AppendFormat("Select Count(*) as {1} from ({0}) x", query.Sql, resultName);
            query.Sql = b.ToString();
            return query;
        }

        public static DbQuery PaginationSelect<T>(Dictionary<string, object> conds, string orderColumn, OrderType orderType, int start, int count)
        {
            return PaginationSelect<T>(SimpleSelect<T>(conds), orderColumn, orderType, start, count);
        }

        public static DbQuery PaginationSelect<T>(DbQuery innerSelect, string orderColumn, OrderType orderType, int start, int count)
        {
            var b = new StringBuilder();
            b.AppendFormat("select count(*) as AllCount from ({0}) x;", innerSelect.Sql);
            b.AppendFormat("select x.* from ({0}) x ", innerSelect.Sql);
            b.AppendFormat(" order by x.{0} {1}", orderColumn, orederTypesMap[orderType]);
            b.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", start, count);
            innerSelect.Sql = b.ToString();
            return innerSelect;
        }

    }


    public class DbQuery
    {
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 
    }
}
